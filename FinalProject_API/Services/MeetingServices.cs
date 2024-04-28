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

namespace FinalProject_API.Services
{
    public interface IMeetingServices
    {
        Task<string> CreateForm(MeetingFormCreating creating, string actor_id);
        Task<MeetingForm> GetForm(string form_id, string actor_id);
        Task<List<MeetingForm>> GetAllForm(string actor_id);
        Task<List<MeetingForm>> SearchForm(MeetingFormSearching searching, string actor_id);
        Task<bool> Delete(string id, string actor_id);
        Task<bool> VoteForm(MeetingFormVoting voting);
        Task<bool> BookMeeting(string form_id, string actor_id);
    }
    public class MeetingServices : IMeetingServices
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;

        public MeetingServices(
            DatabaseContext context,
            IMapper mapper
            )
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<string> CreateForm(MeetingFormCreating creating, string actor_id)
        {
            var new_meeting_form = new MeetingForm();
            new_meeting_form.ID = SlugID.New();
            new_meeting_form.URL = "<invite_url>";
            new_meeting_form.trangthai = (int)trangthai_MeetingForm.Moi;
            new_meeting_form.meeting_title = creating.meeting_title;
            new_meeting_form.meeting_description = creating.meeting_description;
            new_meeting_form.location = creating.location;
            new_meeting_form.platform = creating.platform;
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

        public async Task<MeetingForm> GetForm(string form_id, string actor_id)
        {
            var form = await _context.meetingforms.FirstOrDefaultAsync(o => o.ID == form_id);
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
            var forms = await _context.meetingforms.Where(o => o.owner_id == actor_id).ToListAsync();
            return forms;
        }

        public async Task<List<MeetingForm>> SearchForm(MeetingFormSearching searching, string actor_id)
        {
            var query = _context.meetingforms
                        .Where(o => o.owner_id == actor_id)
                        .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searching.meeting_title))
                query = query.Where(k => k.meeting_title.Contains(searching.meeting_title));

            return await query.ToListAsync();
        }

        public async Task<bool> VoteForm(MeetingFormVoting voting)
        {
            var new_attendee = new Attendee();
            new_attendee.ID = SlugID.New();
            new_attendee.email = voting.email;
            new_attendee.username = voting.username;

            var new_attendee_meetingform = new Attendee_MeetingForm();
            new_attendee_meetingform.attendee_id = new_attendee.ID;
            new_attendee_meetingform.meetingform_id = voting.meetingform_id;

            var times = await _context.meetingtimes.Where(o => voting.meetingtime_ids.Contains(o.ID)).ToListAsync();
            foreach (var time in times)
            {
                time.vote_count += 1;
            }
            try
            {
                _context.attendees.Add(new_attendee);
                _context.attendee_meetingforms.Add(new_attendee_meetingform);
                _context.meetingtimes.UpdateRange(times);

                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                throw new InvalidProgramException("Gặp lỗi trong quá trình xử lý");
            }
        }

        public async Task<bool> BookMeeting(string form_id, string actor_id)
        {
            var form = await _context.meetingforms.Include(o => o.attendee_meetingforms).FirstOrDefaultAsync(o => o.ID == form_id);
            var prefered_time = await _context.meetingtimes.Where(o => o.meetingform_id == form_id).OrderByDescending(o => o.vote_count).FirstOrDefaultAsync();

            form.starttime = prefered_time.time;
            form.duration = prefered_time.duration;
            form.trangthai = (int)trangthai_MeetingForm.KetThuc;

            var meeting = _mapper.Map<MeetingForm, Meeting>(form);
            meeting.starttime = form.starttime ?? DateTime.Now;
            meeting.duration = form.duration ?? 60;
            meeting.ID = SlugID.New();
            meeting.trangthai = (int)trangthai_Meeting.Moi;

            foreach(var attendee in form.attendee_meetingforms)
            {
                var attendee_meeting = new Attendee_Meeting();
                attendee_meeting.attendee_id = attendee.attendee_id;
                attendee_meeting.meeting_id = meeting.ID;
                _context.attendee_meetings.Add(attendee_meeting);
            }
            _context.meetings.Add(meeting);

            await _context.SaveChangesAsync();

            return true;
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