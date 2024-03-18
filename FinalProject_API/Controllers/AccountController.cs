using FinalProject_API.Services;
using FinalProject_Data.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject_API.Controllers
{
    [Route("account")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IAccountServices _accountServices;
        public AccountController(IAccountServices accountServices)
        {
            _accountServices = accountServices;
        }
        [HttpGet("get")]
        public ActionResult Get(int id)
        {
            try
            {
                var account = _accountServices.Get(id);
                return Ok(account);
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPost("create")]
        public ActionResult Create([FromBody] Account account)
        {
            try
            {
                var new_account = _accountServices.Create(account);
                return Ok(new_account);
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpDelete("delete")]
        public ActionResult Delete(int id)
        {
            try
            {
                _accountServices.Delete(id);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPut("edit")]
        public ActionResult Edit(int id, string password, string phonenumber, string address)
        {
            try
            {
                var edit_account = _accountServices.Edit(id, password, phonenumber, address);
                return Ok(edit_account);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
