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
    }
}
