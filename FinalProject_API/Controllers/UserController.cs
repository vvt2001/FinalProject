﻿using FinalProject_API.Services;
using FinalProject_API.View.Meeting;
using FinalProject_API.View.User;
using FinalProject_API.Wrappers;
using FinalProject_Data.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject_API.Controllers
{
    [Route("user")]
    [ApiController]
    public class UserController : ApiControllerBase
    {
        private readonly IUserServices _userServices;
        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [HttpPost("create")]
        public async Task<ActionResult> Create([FromBody] UserCreating creating)
        {
            try
            {
                return Ok(new Response<string>(await _userServices.Create(creating)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [Authorize]
        [HttpPut("Update")]
        public async Task<ActionResult> Update([FromBody] UserUpdating updating, string actor_id)
        {
            try
            {
                return Ok(new Response<bool>(await _userServices.Update(updating, actor_id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult> Get(string id, string actor_id)
        {
            try
            {
                return Ok(new Response<User>(await _userServices.Get(id, actor_id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult> Authenticate([FromBody] LoginRequest request)
        {
            try
            {
                return Ok(new Response<LoginResponse>(await _userServices.Authenticate(request)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [Authorize]
        [HttpDelete("remove-credentials")]
        public async Task<ActionResult> RemoveCredentials(string user_id)
        {
            try
            {
                return Ok(new Response<bool>(await _userServices.RemoveCredentials(user_id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [Authorize]
        [HttpPost("add-credentials")]
        public async Task<ActionResult> AddCredentials(string user_id)
        {
            try
            {
                return Ok(new Response<bool>(await _userServices.AddCredentials(user_id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
