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

namespace FinalProject_API.Services
{
    public interface IMeetingServices
    {
        Task<Meeting> GetMeeting(string form_id, string actor_id);
        Task<List<Meeting>> GetAllMeeting(string actor_id);
        Task<PagedResponse<List<Meeting>>> SearchMeeting(MeetingSearching searching, string actor_id);
        Task<bool> DeleteMeeting(string id, string actor_id);
        Task<bool> UpdateMeeting(MeetingUpdating updating, string actor_id);
        Task<bool> CancelMeeting(string id, string actor_id);
    }
    public class MeetingServices : IMeetingServices
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;
        private readonly IOnlineMeetingServices _onlineMeetingServices;
        public MeetingServices(
            DatabaseContext context,
            IMapper mapper,
            IOnlineMeetingServices onlineMeetingServices
        )
        {
            _context = context;
            _mapper = mapper;
            _onlineMeetingServices = onlineMeetingServices;
        }

        public async Task<bool> UpdateMeeting(MeetingUpdating updating, string actor_id)
        {
            if (updating.attendees != null && updating.attendees.GroupBy(a => a.email).Any(g => g.Count() > 1))
            {
                throw new InvalidProgramException("Không được phép trùng email");
            }

            var meeting = await GetMeeting(updating.id, actor_id);

            meeting.meeting_title = updating.meeting_title;
            meeting.meeting_description = updating.meeting_description;
            meeting.location = updating.location;
            meeting.duration = updating.duration;
            meeting.starttime = updating.starttime;

            var oldAttendees = await _context.attendees.Where(o => o.meeting_id == updating.id).ToListAsync();
            var oldAttendees_emails = oldAttendees.Select(o => o.email).ToList();

            var deletedAttendees =  oldAttendees.Where(o => !updating.attendees.Select(t => t.email).ToList().Contains(o.email)).ToList();
            _context.attendees.RemoveRange(deletedAttendees);

            if (updating.attendees != null)
            {
                var newAttendees = updating.attendees.Where(o => !oldAttendees_emails.Contains(o.email)).ToList();
                foreach (var attendee in newAttendees)
                {
                    var new_attendee = new Attendee();

                    new_attendee.ID = SlugID.New();
                    new_attendee.email = attendee.email;
                    new_attendee.name = attendee.name;
                    new_attendee.meeting_id = meeting.ID;
                    new_attendee.meetingform_id = meeting.meetingform_id;

                    _context.attendees.Add(new_attendee);
                }

                var nameChangedAttendee = updating.attendees.Where(o => oldAttendees_emails.Contains(o.email)).ToList();
                foreach (var attendee in nameChangedAttendee)
                {
                    var updateAttendee = await _context.attendees.FirstOrDefaultAsync(o => o.email == attendee.email);
                    if (updateAttendee != null)
                    {
                        updateAttendee.email = attendee.email;
                        updateAttendee.name = attendee.name;

                        _context.attendees.Update(updateAttendee);
                    }
                }
            }

            _context.meetings.Update(meeting);
            await _context.SaveChangesAsync();

            if (string.IsNullOrWhiteSpace(meeting.event_id))
            {
                throw new InvalidProgramException("Không tìm thấy thông tin lịch họp");
            }
            await _onlineMeetingServices.UpdateGoogleMeetMeeting(meeting.event_id, actor_id, meeting);

            return true;
        }

        public async Task<Meeting> GetMeeting(string form_id, string actor_id)
        {
            var meeting = await _context.meetings.Include(o => o.attendees).FirstOrDefaultAsync(o => o.ID == form_id);
            if(meeting != null)
            {
                return meeting;
            }
            else
            {
                throw new InvalidProgramException("Không tìm thấy cuộc họp");
            }
        }

        public async Task<List<Meeting>> GetAllMeeting(string actor_id)
        {
            var meetings = await _context.meetings.Include(o => o.attendees).Where(o => o.owner_id == actor_id).ToListAsync();
            return meetings;
        }

        public async Task<PagedResponse<List<Meeting>>> SearchMeeting(MeetingSearching searching, string actor_id)
        {
            var pagedDatas = new PagedResponse<List<Meeting>>
            {
                PageNumber = searching.PageNumber,
                PageSize = searching.PageSize
            };

            var query = _context.meetings
                        .Include(o => o.attendees)
                        .Where(o => o.owner_id == actor_id)
                        .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searching.meeting_title))
                query = query.Where(k => k.meeting_title.Contains(searching.meeting_title));

            pagedDatas.TotalRecords = await query.CountAsync();

            pagedDatas.Data = await query.Paged(searching.PageNumber, searching.PageSize).ToListAsync();
            pagedDatas.Succeed = true;
            return pagedDatas;
        }

        public async Task<bool> DeleteMeeting(string id, string actor_id)
        {
            var meeting = await GetMeeting(id, actor_id);
            var meetingEventID = meeting.event_id;
            var attendees = await _context.attendees.Where(a => a.meeting_id == id).ToListAsync();
            attendees.ForEach(a => a.meeting_id = null);
            await _context.SaveChangesAsync();
            await _context.meetings.Where(o => o.ID == id).ExecuteDeleteAsync();
            if (string.IsNullOrWhiteSpace(meetingEventID))
            {
                throw new InvalidProgramException("Không tìm thấy thông tin lịch họp");
            }
            await _onlineMeetingServices.DeleteGoogleMeetMeeting(meetingEventID, actor_id);
            return true;
        }

        public async Task<bool> CancelMeeting(string id, string actor_id)
        {
            var meeting = await GetMeeting(id, actor_id);
            if (string.IsNullOrWhiteSpace(meeting.event_id))
            {
                throw new InvalidProgramException("Không tìm thấy thông tin lịch họp");
            }
            await _onlineMeetingServices.CancelGoogleMeetMeeting(meeting.event_id, actor_id);
            meeting.trangthai = (int)trangthai_Meeting.Canceled;
            _context.meetings.Update(meeting);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
