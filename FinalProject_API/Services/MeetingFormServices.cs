using FinalProject_API.View.MeetingForm;
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
using Google.Apis.Calendar.v3.Data;
using System.Collections;
using Microsoft.Extensions.Configuration;

namespace FinalProject_API.Services
{
    public interface IMeetingFormServices
    {
        Task<string> CreateForm(MeetingFormCreating creating, string actor_id);
        Task<MeetingForm> GetForm(string form_id, string actor_id);
        Task<List<MeetingForm>> GetAllForm(string actor_id);
        Task<PagedResponse<List<MeetingForm>>> SearchForm(MeetingFormSearching searching, string actor_id);
        Task<bool> DeleteForm(string id, string actor_id);
        Task<bool> VoteForm(MeetingFormVoting voting);
        Task<bool> BookMeeting(MeetingFormBooking booking, string actor_id);
        Task<bool> UpdateForm(MeetingFormUpdating updating, string actor_id);
    }
    public class MeetingFormServices : IMeetingFormServices
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;
        private readonly IOnlineMeetingServices _onlineMeetingServices;
        private readonly IMeetingServices _meetingServices;
        private readonly IConfiguration _configuration;

        public MeetingFormServices(
            DatabaseContext context,
            IMapper mapper,
            IOnlineMeetingServices onlineMeetingServices,
            IMeetingServices meetingServices,
            IConfiguration configuration
        )
        {
            _context = context;
            _mapper = mapper;
            _onlineMeetingServices = onlineMeetingServices;
            _meetingServices = meetingServices;
            _configuration = configuration;
        }

        public async Task<string> CreateForm(MeetingFormCreating creating, string actor_id)
        {
            var credentials = await _onlineMeetingServices.GetCredentialCalendar(actor_id);
            if (credentials == null)
            {
                throw new InvalidProgramException("Google credentials invalid");
            }
            if (creating.times == null || creating.times.Count() < 2)
            {
                throw new InvalidProgramException("Must add atleast 2 meeting times");
            }
            if (creating.times.Any(d => d < DateTime.Now))
            {
                throw new InvalidProgramException("Can't select past times");
            }
            var duplicates = creating.times
                .GroupBy(d => d)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Any())
            {
                // Handle the error condition, e.g., throw an exception
                throw new InvalidProgramException("Can't select duplicated times");
            }
            var new_meeting_form = new MeetingForm();
            new_meeting_form.ID = SlugID.New();
            new_meeting_form.URL = $"http://localhost:3000/guest/{new_meeting_form.ID}/vote";
            new_meeting_form.trangthai = (int)trangthai_MeetingForm.New;
            new_meeting_form.meeting_title = creating.meeting_title;
            new_meeting_form.meeting_description = creating.meeting_description;
            new_meeting_form.location = creating.location;
            new_meeting_form.platform = creating.platform;
            new_meeting_form.duration = creating.duration;
            new_meeting_form.owner_id = actor_id;

            var meetingtimes = new List<MeetingTime>();

            foreach (var time in creating.times)
            {
                var meetingtime = new MeetingTime();

                meetingtime.ID = SlugID.New();
                meetingtime.meetingform_id = new_meeting_form.ID;
                meetingtime.time = time;
                meetingtime.duration = creating.duration;
                meetingtime.trangthai = (int)trangthai_MeetingTime.New;

                meetingtimes.Add(meetingtime);

                _context.meetingtimes.Add(meetingtime);
            }

            new_meeting_form.times = meetingtimes;
            _context.meetingforms.Add(new_meeting_form);

            await _context.SaveChangesAsync();

            return new_meeting_form.ID;
        }

        public async Task<bool> UpdateForm(MeetingFormUpdating updating, string actor_id)
        {
            if (updating.times == null || updating.times.Count() < 2)
            {
                throw new InvalidProgramException("Must add atleast 2 meeting times");
            }
            if (updating.times.Any(d => d < DateTime.Now))
            {
                throw new InvalidProgramException("Can't select past times");
            }
            var duplicates = updating.times
                .GroupBy(d => d)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Any())
            {
                throw new InvalidProgramException("Can't select duplicated times");
            }

            var meetingForm = await _context.meetingforms.AsNoTracking().FirstOrDefaultAsync(o => o.ID == updating.id);

            if (meetingForm != null)
            {
                meetingForm.meeting_title = updating.meeting_title;
                meetingForm.meeting_description = updating.meeting_description;
                meetingForm.location = updating.location;
                meetingForm.platform = updating.platform;
                meetingForm.duration = updating.duration;
                meetingForm.owner_id = actor_id;

                var oldMeetingTimes = await _context.meetingtimes.Where(o => o.meetingform_id == updating.id).ToListAsync();
                _context.meetingtimes.RemoveRange(oldMeetingTimes);

                var removed_history = await _context.votinghistories.Where(o => oldMeetingTimes.Select(o => o.ID).ToList().Contains(o.meetingtime_id)).ToListAsync();
                _context.votinghistories.RemoveRange(removed_history);

                var meetingtimes = new List<MeetingTime>();
                foreach (var time in updating.times)
                {
                    var meetingtime = new MeetingTime();

                    meetingtime.ID = SlugID.New();
                    meetingtime.meetingform_id = meetingForm.ID;
                    meetingtime.time = time;
                    meetingtime.duration = updating.duration;
                    meetingtime.trangthai = (int)trangthai_MeetingTime.New;

                    meetingtimes.Add(meetingtime);

                    _context.meetingtimes.Add(meetingtime);
                }

                meetingForm.times = meetingtimes;

                _context.meetingforms.Update(meetingForm);
                return await _context.SaveChangesAsync() > 0;
            }
            throw new InvalidProgramException("Can't find meeting schedule");
        }

        public async Task<MeetingForm> GetForm(string form_id, string actor_id)
        {
            var form = await _context.meetingforms.Include(o => o.times).Include(o => o.attendees).Include(o => o.owner).FirstOrDefaultAsync(o => o.ID == form_id);
            if(form != null)
            {
                return form;
            }
            else
            {
                throw new InvalidProgramException("Can't find meeting schedule");
            }
        }

        public async Task<List<MeetingForm>> GetAllForm(string actor_id)
        {
            var forms = await _context.meetingforms.Include(o => o.attendees).Where(o => o.owner_id == actor_id).ToListAsync();
            return forms;
        }

        public async Task<PagedResponse<List<MeetingForm>>> SearchForm(MeetingFormSearching searching, string actor_id)
        {
            var pagedDatas = new PagedResponse<List<MeetingForm>>
            {
                PageNumber = searching.PageNumber,
                PageSize = searching.PageSize
            };

            var query = _context.meetingforms
                        .Include(o => o.attendees)
                        .Include(o => o.times)
                        .Where(o => o.owner_id == actor_id)
                        .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searching.meeting_title))
                query = query.Where(k => k.meeting_title.Contains(searching.meeting_title));

            pagedDatas.TotalRecords = await query.CountAsync();

            pagedDatas.Data = await query.Paged(searching.PageNumber, searching.PageSize).ToListAsync();
            pagedDatas.Succeed = true;
            return pagedDatas;
        }

        public async Task<bool> VoteForm(MeetingFormVoting voting)
        {
            var attendees = await _context.attendees.Where(o => o.meetingform_id == voting.meetingform_id && o.email == voting.email).FirstOrDefaultAsync();
            var meeting_form = await _context.meetingforms.Include(o => o.times).Include(o => o.attendees).Include(o => o.owner).FirstOrDefaultAsync(o => o.ID == voting.meetingform_id);
            var voted_times = await _context.meetingtimes.Where(o => voting.meetingtime_ids.Contains(o.ID)).ToListAsync();
            if (meeting_form == null)
            {
                throw new InvalidProgramException("Meeting schedule not found");
            }
            if (attendees != null)
            {
                attendees.name = voting.name;
                _context.attendees.Update(attendees);

                if (voting.meetingtime_ids == null || voting.meetingtime_ids.Count < 1)
                {
                    throw new InvalidProgramException("Must vote for atleast 1 meeting time(s)");
                }

                var voting_history = await _context.votinghistories.Where(o => o.attendee_id == attendees.ID && o.meetingform_id == voting.meetingform_id).ToListAsync();
                var removed_voting_history = voting_history.Where(o => !voting.meetingtime_ids.Contains(o.meetingtime_id)).ToList();
                _context.votinghistories.RemoveRange(removed_voting_history);

                foreach (var meetingtime_id in voting.meetingtime_ids.Except(voting_history.Select(o => o.meetingtime_id).ToList()).ToList())
                {
                    var new_voting_history = new VotingHistory
                    {
                        ID = SlugID.New(),
                        meetingtime_id = meetingtime_id,
                        meetingform_id = voting.meetingform_id,
                        attendee_id = attendees.ID
                    };

                    _context.votinghistories.Add(new_voting_history);
                }

                await _context.SaveChangesAsync();

                var meeting_times = await _context.meetingtimes.Where(o => o.meetingform_id == voting.meetingform_id).ToListAsync();
                foreach (var time in meeting_times)
                {
                    time.vote_count = _context.votinghistories.Where(o => o.meetingform_id == voting.meetingform_id && o.meetingtime_id == time.ID).Count();
                    _context.meetingtimes.Update(time);
                }

                await _context.SaveChangesAsync();

                //Send email
                var voted_times_string = voted_times.Select(o => o.time.ToString("dd/MM/yyyy HH:mm")).ToList();
                var most_voted_time = meeting_form.times.OrderByDescending(o => o.vote_count).FirstOrDefault();
                if (most_voted_time != null && voted_times_string.Count > 0)
                {
                    await _onlineMeetingServices.SendSystemEmail(meeting_form, $"Meeting '{meeting_form.meeting_title}': New attendee has voted for a meeting time", $"A new attendee has voted for the meeting times {string.Join(", ", voted_times_string)}.\nCurrently, the meeting are scheduled to be held at {most_voted_time.time.ToString("dd/MM/yyyy HH:mm")} with {most_voted_time.vote_count} votes.\nCheck the meeting's info here {meeting_form.URL}");
                }
            }
            else
            {
                meeting_form.trangthai = (int)trangthai_MeetingForm.Voting;

                if (voting.meetingtime_ids == null || voting.meetingtime_ids.Count < 1)
                {
                    throw new InvalidProgramException("Must vote for atleast 1 meeting time(s)");
                }
                var new_attendee = new Attendee();
                new_attendee.ID = SlugID.New();
                new_attendee.email = voting.email;
                new_attendee.name = voting.name;
                new_attendee.meetingform_id = voting.meetingform_id;

                _context.attendees.Add(new_attendee);
                _context.meetingforms.Update(meeting_form);

                foreach (var meetingtime_id in voting.meetingtime_ids)
                {
                    var new_voting_history = new VotingHistory
                    {
                        ID = SlugID.New(),
                        meetingtime_id = meetingtime_id,
                        meetingform_id = voting.meetingform_id,
                        attendee_id = new_attendee.ID
                    };

                    _context.votinghistories.Add(new_voting_history);
                }

                await _context.SaveChangesAsync();

                var meeting_times = await _context.meetingtimes.Where(o => o.meetingform_id == voting.meetingform_id).ToListAsync();
                foreach (var time in meeting_times)
                {
                    time.vote_count = _context.votinghistories.Where(o => o.meetingform_id == voting.meetingform_id && o.meetingtime_id == time.ID).Count();
                    _context.meetingtimes.Update(time);
                }

                await _context.SaveChangesAsync();

                //Send email
                var voted_times_string = voted_times.Select(o => o.time.ToString("dd/MM/yyyy HH:mm")).ToList();
                var most_voted_time = meeting_form.times.OrderByDescending(o => o.vote_count).FirstOrDefault();
                if (most_voted_time != null && voted_times_string.Count > 0)
                {
                    await _onlineMeetingServices.SendSystemEmail(meeting_form, $"Meeting '{meeting_form.meeting_title}': New attendee has voted for a meeting time", $"A new attendee has voted for the meeting times {string.Join(", ", voted_times_string)}.\nCurrently, the meeting are scheduled to be held at {most_voted_time.time.ToString("dd/MM/yyyy HH:mm")} with {most_voted_time.vote_count} votes.\nCheck the meeting's info here {meeting_form.URL}");
                }
            }
            return true;
        }

        public async Task<bool> BookMeeting(MeetingFormBooking booking, string actor_id)
        {
            var form = await _context.meetingforms.Include(o => o.attendees).FirstOrDefaultAsync(o => o.ID == booking.meetingform_id);
            var prefered_time = await _context.meetingtimes.Where(o => o.meetingform_id == booking.meetingform_id).OrderByDescending(o => o.vote_count).FirstOrDefaultAsync();

            form.starttime = prefered_time.time;
            form.duration = prefered_time.duration;
            form.trangthai = (int)trangthai_MeetingForm.Ended;

            var meeting = _mapper.Map<MeetingForm, Meeting>(form);
            meeting.starttime = form.starttime ?? DateTime.Now;
            meeting.duration = form.duration;
            meeting.ID = SlugID.New();
            meeting.trangthai = (int)trangthai_Meeting.Waiting;
            meeting.meetingform_id = booking.meetingform_id;

            foreach (var attendee in form.attendees)
            {
                attendee.meeting_id = meeting.ID;
                _context.attendees.Update(attendee);
            }
            _context.meetings.Add(meeting);

            await _context.SaveChangesAsync();

            var createdMeeting = await _meetingServices.GetMeeting(meeting.ID, actor_id);
            await _onlineMeetingServices.CreateGoogleMeetMeeting(createdMeeting, actor_id);

            return true;
        }

        public async Task<bool> DeleteForm(string id, string actor_id)
        {
            var form = await _context.meetingforms.FirstOrDefaultAsync(o => o.ID == id);
            if (form != null)
            {
                var attendees = await _context.attendees.Where(a => a.meetingform_id == id).ToListAsync();
                attendees.ForEach(a => a.meetingform_id = null);

                await _context.SaveChangesAsync();
                await _context.meetingforms.Where(o => o.ID == id).ExecuteDeleteAsync();
                await _context.attendees.Where(o => o.meetingform_id == null && o.meeting_id == null ).ExecuteDeleteAsync();

                return true;
            }
            throw new InvalidProgramException("Meeting schedule not found");
        }
    }
}
