using System;
using System.Collections.Generic;

namespace FinalProject_API.View.User
{
    public class LoginResponse
    {
        public string id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string access_token { get; set; }
        public bool has_googlecredentials { get; set; }
    }
}

