﻿using FinalProject_API.Services;
using FinalProject_API.View.Meeting;
using FinalProject_API.Wrappers;
using FinalProject_Data.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject_API.Controllers
{
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
