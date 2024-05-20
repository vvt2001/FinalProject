using FinalProject_API.View.Meeting;
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


            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

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
                Attendees = new List<EventAttendee>
                {
                    new EventAttendee { Email = "vvt69420@gmail.com" },
                    new EventAttendee { Email = "thang.vv194374@sis.hust.edu.vn" }
                },
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

            return true;
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
    }
}
