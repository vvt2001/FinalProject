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
        Task<string> CreateForm(MeetingFormCreating creating, string actor_id);
        Task<MeetingForm> GetForm(string form_id, string actor_id);
        Task<List<MeetingForm>> GetAllForm(string actor_id);
        Task<PagedResponse<List<MeetingForm>>> SearchForm(MeetingFormSearching searching, string actor_id);
        Task<bool> Delete(string id, string actor_id);
        Task<bool> VoteForm(MeetingFormVoting voting);
        Task<bool> BookMeeting(MeetingFormBooking booking, string actor_id);
        Task<bool> UpdateForm(MeetingFormUpdating updating, string actor_id);
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

        public async Task<string> CreateForm(MeetingFormCreating creating, string actor_id)
        {
            var new_meeting_form = new MeetingForm();
            new_meeting_form.ID = SlugID.New();
            new_meeting_form.URL = $"http://localhost:3000/guest/{new_meeting_form.ID}/vote";
            new_meeting_form.trangthai = (int)trangthai_MeetingForm.Moi;
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
                meetingtime.trangthai = (int)trangthai_MeetingTime.moi;

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

                var meetingtimes = new List<MeetingTime>();
                foreach (var time in updating.times)
                {
                    var meetingtime = new MeetingTime();

                    meetingtime.ID = SlugID.New();
                    meetingtime.meetingform_id = meetingForm.ID;
                    meetingtime.time = time;
                    meetingtime.duration = updating.duration;
                    meetingtime.trangthai = (int)trangthai_MeetingTime.moi;

                    meetingtimes.Add(meetingtime);

                    _context.meetingtimes.Add(meetingtime);
                }

                meetingForm.times = meetingtimes;

                _context.meetingforms.Update(meetingForm);
                return await _context.SaveChangesAsync() > 0;
            }
            throw new InvalidProgramException("Cuộc họp không tồn tại");
        }

        public async Task<MeetingForm> GetForm(string form_id, string actor_id)
        {
            var form = await _context.meetingforms.Include(o => o.times).Include(o => o.attendee).FirstOrDefaultAsync(o => o.ID == form_id);
            if(form != null)
            {
                return form;
            }
            else
            {
                throw new InvalidProgramException("Không tìm thấy lịch họp");
            }
        }

        public async Task<List<MeetingForm>> GetAllForm(string actor_id)
        {
            var forms = await _context.meetingforms.Include(o => o.attendee).Where(o => o.owner_id == actor_id).ToListAsync();
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
                        .Include(o => o.attendee)
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
            var attendee = _context.attendees.Where(o => o.meetingform_id == voting.meetingform_id && o.email == voting.email);
            if(attendee.Count() > 0)
            {
                throw new InvalidProgramException("Email đã được sử dụng cho cuộc họp này");
            }
            var new_attendee = new Attendee();
            new_attendee.ID = SlugID.New();
            new_attendee.email = voting.email;
            new_attendee.name = voting.name;
            new_attendee.meetingform_id = voting.meetingform_id;

            var times = await _context.meetingtimes.Where(o => voting.meetingtime_ids.Contains(o.ID)).ToListAsync();
            foreach (var time in times)
            {
                time.vote_count += 1;
            }
            try
            {
                _context.attendees.Add(new_attendee);
                _context.meetingtimes.UpdateRange(times);

                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                throw new InvalidProgramException("Gặp lỗi trong quá trình xử lý");
            }
        }

        public async Task<bool> BookMeeting(MeetingFormBooking booking, string actor_id)
        {
            var form = await _context.meetingforms.Include(o => o.attendee).FirstOrDefaultAsync(o => o.ID == booking.meetingform_id);
            var prefered_time = await _context.meetingtimes.Where(o => o.meetingform_id == booking.meetingform_id).OrderByDescending(o => o.vote_count).FirstOrDefaultAsync();

            form.starttime = prefered_time.time;
            form.duration = prefered_time.duration;
            form.trangthai = (int)trangthai_MeetingForm.KetThuc;

            var meeting = _mapper.Map<MeetingForm, Meeting>(form);
            meeting.starttime = form.starttime ?? DateTime.Now;
            meeting.duration = form.duration;
            meeting.ID = SlugID.New();
            meeting.trangthai = (int)trangthai_Meeting.Moi;

            foreach(var attendee in form.attendee)
            {
                attendee.meeting_id = meeting.ID;
                _context.attendees.Update(attendee);
            }
            _context.meetings.Add(meeting);

            await _context.SaveChangesAsync();

            return await _onlineMeetingServices.CreateGoogleMeetMeeting(meeting, actor_id);
        }

        public async Task<bool> Delete(string id, string actor_id)
        {
            var form = await _context.meetingforms.FirstOrDefaultAsync(o => o.ID == id);
            if (form != null)
            {
                await _context.meetingtimes.Where(o => o.meetingform_id == id).ExecuteDeleteAsync();
                await _context.meetingforms.Where(o => o.ID == id).ExecuteDeleteAsync();
                return true;
            }
            throw new InvalidProgramException("Mẫu cuộc họp không tồn tại");
        }
    }
}
