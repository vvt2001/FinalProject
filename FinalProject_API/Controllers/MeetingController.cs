using FinalProject_API.Services;
using FinalProject_API.View.Meeting;
using FinalProject_API.Wrappers;
using FinalProject_Data.Enum;
using FinalProject_Data.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject_API.Controllers
{
    //[Authorize]
    [Route("meeting")]
    [ApiController]
    public class MeetingController : ApiControllerBase
    {
        private readonly IMeetingServices _meetingServices;
        public MeetingController(IMeetingServices meetingServices)
        {
            _meetingServices = meetingServices;
        }

        [Authorize]
        [HttpPut("update-meeting")]
        public async Task<ActionResult> UpdateMeeting([FromBody] MeetingUpdating updating, string actor_id)
        {
            try
            {
                return Ok(new Response<bool>(await _meetingServices.UpdateMeeting(updating, actor_id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("get-meeting/{meeting_id}")]
        public async Task<ActionResult> GetMeeting(string meeting_id, string actor_id)
        {
            try
            {
                return Ok(new Response<Meeting>(await _meetingServices.GetMeeting(meeting_id, actor_id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("get-all-meeting")]
        public async Task<ActionResult> GetAllMeeting(string actor_id)
        {
            try
            {
                return Ok(new Response<List<Meeting>>(await _meetingServices.GetAllMeeting(actor_id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("search-meeting")]
        public async Task<ActionResult> SearchMeeting([FromQuery] MeetingSearching request, string actor_id)
        {
            try
            {
                return Ok(await _meetingServices.SearchMeeting(request, actor_id));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [Authorize]
        [HttpDelete("delete-meeting/{id}")]
        public async Task<ActionResult> DeleteMeeting(string id, string actor_id)
        {
            try
            {
                return Ok(new Response<bool>(await _meetingServices.DeleteMeeting(id, actor_id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [Authorize]
        [HttpPut("cancel-meeting/{id}")]
        public async Task<ActionResult> CancelMeeting(string id, string actor_id)
        {
            try
            {
                return Ok(new Response<bool>(await _meetingServices.CancelMeeting(id, actor_id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [Authorize]
        [HttpPut("add-note")]
        public async Task<ActionResult> AddNote([FromBody] MeetingNote note, string actor_id)
        {
            try
            {
                return Ok(new Response<bool>(await _meetingServices.AddNote(note, actor_id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
