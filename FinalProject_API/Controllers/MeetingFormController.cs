using FinalProject_API.Services;
using FinalProject_API.View.MeetingForm;
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
    [Route("meeting-form")]
    [ApiController]
    public class MeetingFormController : ApiControllerBase
    {
        private readonly IMeetingFormServices _meetingServices;
        public MeetingFormController(IMeetingFormServices meetingServices)
        {
            _meetingServices = meetingServices;
        }

        [Authorize]
        [HttpPost("create-form")]
        public async Task<ActionResult> Create([FromBody] MeetingFormCreating creating, string actor_id)
        {
            try
            {
                return Ok(new Response<string>(await _meetingServices.CreateForm(creating, actor_id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [Authorize]
        [HttpPut("update-form")]
        public async Task<ActionResult> Update([FromBody] MeetingFormUpdating updating, string actor_id)
        {
            try
            {
                return Ok(new Response<bool>(await _meetingServices.UpdateForm(updating, actor_id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("get-form/{form_id}")]
        public async Task<ActionResult> GetForm(string form_id, string actor_id)
        {
            try
            {
                return Ok(new Response<MeetingForm>(await _meetingServices.GetForm(form_id, actor_id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("get-all-form")]
        public async Task<ActionResult> GetAllForm(string actor_id)
        {
            try
            {
                return Ok(new Response<List<MeetingForm>>(await _meetingServices.GetAllForm(actor_id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("search-form")]
        public async Task<ActionResult> SearchForm([FromQuery] MeetingFormSearching request, string actor_id)
        {
            try
            {
                return Ok(await _meetingServices.SearchForm(request, actor_id));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [Authorize]
        [HttpDelete("delete-form/{id}")]
        public async Task<ActionResult> DeleteForm(string id, string actor_id)
        {
            try
            {
                return Ok(new Response<bool>(await _meetingServices.DeleteForm(id, actor_id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("vote-form")]
        public async Task<ActionResult> VoteForm([FromBody] MeetingFormVoting voting)
        {
            try
            {
                return Ok(new Response<bool>(await _meetingServices.VoteForm(voting)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [Authorize]
        [HttpPost("book-meeting")]
        public async Task<ActionResult> BookMeeting([FromBody] MeetingFormBooking booking, string actor_id)
        {
            try
            {
                return Ok(new Response<bool>(await _meetingServices.BookMeeting(booking, actor_id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
