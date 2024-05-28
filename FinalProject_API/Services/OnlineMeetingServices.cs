﻿using FinalProject_API.View.Meeting;
using FinalProject_Data;
using FinalProject_Data.Model;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FinalProject_Data.Enum.Meeting;
using FinalProject_API.Common;
using FinalProject_API.Wrappers;
using FinalProject_API.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Calendar.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Flows;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using FinalProject_API.View.GoogleCredentials;

namespace FinalProject_API.Services
{
    public interface IOnlineMeetingServices
    {
        Task<bool> CreateGoogleMeetMeeting(Meeting meeting, string actor_id);
        Task<bool> CancelGoogleMeetMeeting(string eventId, string actor_id);
        Task<bool> DeleteGoogleMeetMeeting(string eventId, string actor_id);
        Task<bool> UpdateGoogleMeetMeeting(string eventId, string actor_id, Meeting new_meeting);
    }
    [Obsolete]
    public class OnlineMeetingServices : IOnlineMeetingServices
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public OnlineMeetingServices(
            DatabaseContext context,
            IMapper mapper,
            IConfiguration configuration
        )
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        static string[] Scopes = { CalendarService.Scope.Calendar };
        static string ApplicationName = "Scheduler";

        public async Task<bool> CreateGoogleMeetMeeting(Meeting meeting, string actor_id)
        {
            UserCredential credential = await GetCredential(actor_id);

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            if (meeting.attendees != null)
            {
                var listEventAttendee = new List<EventAttendee>();
                foreach (var email in meeting.attendees.Select(o => o.email).ToList())
                {
                    var attendee = new EventAttendee() { Email = email };
                    listEventAttendee.Add(attendee);
                }
            }

            // Example: Create an event with a Google Meet link
            var newEvent = new Event
            {
                Summary = meeting.meeting_title,
                Description = meeting.meeting_description,
                Start = new EventDateTime
                {
                    DateTime = meeting.starttime,
                    TimeZone = "Asia/Ho_Chi_Minh"
                },
                End = new EventDateTime
                {
                    DateTime = meeting.starttime.AddMinutes(meeting.duration),
                    TimeZone = "Asia/Ho_Chi_Minh"
                },
                ConferenceData = new ConferenceData
                {
                    CreateRequest = new CreateConferenceRequest
                    {
                        RequestId = Guid.NewGuid().ToString()
                    }
                },
                Attendees = listEventAttendee,
                Reminders = new Event.RemindersData
                {
                    UseDefault = false,
                    Overrides = new EventReminder[] {
                        new EventReminder() { Method = "email", Minutes = 24 * 60 },
                        new EventReminder() { Method = "popup", Minutes = 10 }
                    }
                },
            };

            // Insert the event into the primary calendar
            var request = service.Events.Insert(newEvent, "primary");
            request.ConferenceDataVersion = 1;
            var createdEvent = request.Execute();

            // Output the Google Meet link and the event HTML link
            Console.WriteLine("Event created: {0}", createdEvent.HtmlLink);
            Console.WriteLine("Google Meet link: {0}", createdEvent.ConferenceData.EntryPoints[0].Uri);

            meeting.meeting_link = createdEvent.ConferenceData.EntryPoints[0].Uri;
            meeting.event_id = createdEvent.Id;
            _context.meetings.Update(meeting);
            
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteGoogleMeetMeeting(string eventId, string actor_id)
        {
            UserCredential credential = await GetCredential(actor_id);

            // Create Google Calendar API service
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Delete the event with notifications
            var deleteRequest = service.Events.Delete("primary", eventId);
            deleteRequest.SendUpdates = EventsResource.DeleteRequest.SendUpdatesEnum.All;  // Notify all attendees using enum
            var deleteResponse = await deleteRequest.ExecuteAsync();

            Console.WriteLine("Event deleted with notifications: {0}", eventId);

            return true;
        }


        public async Task<bool> CancelGoogleMeetMeeting(string eventId, string actor_id)
        {
            UserCredential credential = await GetCredential(actor_id);

            // Create Google Calendar API service
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Retrieve the event
            var eventToUpdate = service.Events.Get("primary", eventId).Execute();
            eventToUpdate.Status = "cancelled";  // Mark the event as cancelled
            eventToUpdate.Summary = "[Cancelled] " + eventToUpdate.Summary; // Optional: Update summary to indicate cancellation

            // Update the event
            var updateRequest = service.Events.Update(eventToUpdate, "primary", eventId);
            updateRequest.SendUpdates = EventsResource.UpdateRequest.SendUpdatesEnum.All; // Notify all attendees
            var updatedEvent = updateRequest.Execute();

            Console.WriteLine("Event cancelled: {0}", updatedEvent.HtmlLink);

            return true;
        }

        public async Task<bool> UpdateGoogleMeetMeeting(string eventId, string actor_id, Meeting new_meeting)
        {
            UserCredential credential = await GetCredential(actor_id);

            var service = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Retrieve the existing event
            var eventToUpdate = await service.Events.Get("primary", eventId).ExecuteAsync();

            bool changesMade = false;

            if (eventToUpdate.Summary != new_meeting.meeting_title) { eventToUpdate.Summary = new_meeting.meeting_title; changesMade = true; }
            if (eventToUpdate.Description != new_meeting.meeting_description) { eventToUpdate.Description = new_meeting.meeting_description; changesMade = true; }
            if (eventToUpdate.Start.DateTime != new_meeting.starttime) { eventToUpdate.Start = new EventDateTime() { DateTime = new_meeting.starttime, TimeZone = "Asia/Ho_Chi_Minh" }; changesMade = true; }
            if (eventToUpdate.End.DateTime != new_meeting.starttime.AddMinutes(new_meeting.duration)) { eventToUpdate.End = new EventDateTime() { DateTime = new_meeting.starttime.AddMinutes(new_meeting.duration), TimeZone = "Asia/Ho_Chi_Minh" }; changesMade = true; }

            if (new_meeting.attendees != null)
            {
                if (eventToUpdate.Attendees == null)
                    eventToUpdate.Attendees = new List<EventAttendee>();

                foreach (var email in new_meeting.attendees.Select(o => o.email).ToList())
                {
                    eventToUpdate.Attendees.Add(new EventAttendee { Email = email });
                }
            }

            if (changesMade)
            {
                // Send the update request with notifications
                var updateRequest = service.Events.Update(eventToUpdate, "primary", eventId);
                updateRequest.SendUpdates = EventsResource.UpdateRequest.SendUpdatesEnum.All;
                var updatedEvent = await updateRequest.ExecuteAsync();
                Console.WriteLine("Event updated with notifications: {0}", updatedEvent.HtmlLink);
            }

            return changesMade;
        }

        private async Task<UserCredential> AuthenticateUserAsync()
        {
            return await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GetClientSecrets(),
                Scopes,
                "user",
                CancellationToken.None);
        }

        private async Task<bool> SaveTokensToDatabase(UserCredential credential, string actor_id)
        {
            var token = new GoogleMeetCredentials
            {
                ID = SlugID.New(),
                AccessToken = credential.Token.AccessToken,
                TokenType = credential.Token.TokenType,
                ExpiresIn = (int)credential.Token.ExpiresInSeconds,
                RefreshToken = credential.Token.RefreshToken,
                Scope = credential.Token.Scope,
                Issued = credential.Token.Issued,
                IssuedUtc = credential.Token.IssuedUtc,
                user_id = actor_id,
            };
            _context.googlemeetcredentials.Add(token);
            return await _context.SaveChangesAsync() > 0;
        }

        private async Task<GoogleMeetCredentials?> GetTokensFromDatabase(string actor_id)
        {
            return await _context.googlemeetcredentials.FirstOrDefaultAsync(t => t.user_id == actor_id);
        }

        private ClientSecrets GetClientSecrets()
        {
            // Get the Google credentials from the configuration
            var googleCredentials = _configuration.GetSection("GoogleCredentials").Get<GoogleCredentials>();

            if (googleCredentials?.web == null)
            {
                throw new InvalidOperationException("Google credentials are not configured correctly in appsettings.json.");
            }

            // Create the client secrets object from the configuration
            var clientSecrets = new ClientSecrets
            {
                ClientId = googleCredentials.web.client_id,
                ClientSecret = googleCredentials.web.client_secret
            };

            return clientSecrets;
        }

        private async Task<UserCredential> GetCredential(string actor_id)
        {
            UserCredential credential;

            // Check if tokens are already stored in the database
            var storedToken = await GetTokensFromDatabase(actor_id);

            if (storedToken != null)
            {
                // Create a TokenResponse using the stored token
                var tokenResponse = new TokenResponse
                {
                    AccessToken = storedToken.AccessToken,
                    RefreshToken = storedToken.RefreshToken,
                    Scope = storedToken.Scope,
                    TokenType = storedToken.TokenType,
                    ExpiresInSeconds = (long)storedToken.ExpiresIn,
                    Issued = storedToken.Issued,
                    IssuedUtc = storedToken.IssuedUtc
                };

                // Create a Flow instance using client secrets
                var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = GetClientSecrets(),
                    Scopes = Scopes
                });

                // Create UserCredential
                credential = new UserCredential(flow, "user", tokenResponse);

                // Refresh the token if it has expired
                if (credential.Token.IsExpired(flow.Clock))
                {
                    if (await credential.RefreshTokenAsync(CancellationToken.None))
                    {
                        await SaveTokensToDatabase(credential, actor_id);
                    }
                    else
                    {
                        credential = await AuthenticateUserAsync();
                        await SaveTokensToDatabase(credential, actor_id);
                    }
                }
            }
            else
            {
                // Authenticate the user and save the token to the database
                credential = await AuthenticateUserAsync();
                await SaveTokensToDatabase(credential, actor_id);
            }

            return credential;
        }
    }
}
