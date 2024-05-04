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

        [HttpDelete("delete-form/{id}")]
        public async Task<ActionResult> Delete(string id, string actor_id)
        {
            try
            {
                return Ok(new Response<bool>(await _meetingServices.Delete(id, actor_id)));
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

        [HttpPost("book-meeting")]
        public async Task<ActionResult> BookMeeting(string form_id, string actor_id)
        {
            try
            {
                return Ok(new Response<bool>(await _meetingServices.BookMeeting(form_id, actor_id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
