using FinalProject_API.Services;
using FinalProject_API.View.MeetingForm;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IUserServices _userServices;
    private readonly IMeetingFormServices _meetingFormServices;

    public AuthController(IConfiguration configuration, IUserServices userServices, IMeetingFormServices meetingFormServices)
    {
        _configuration = configuration;
        _userServices = userServices;
        _meetingFormServices = meetingFormServices;
    }

    [HttpGet("google/callback")]
    public async Task<IActionResult> GoogleCallback(string code, string state)
    {
        if (string.IsNullOrEmpty(code))
        {
            return BadRequest("Authorization code is missing.");
        }

        var clientSecrets = new ClientSecrets
        {
            ClientId = _configuration["GoogleAuth:ClientId"],
            ClientSecret = _configuration["GoogleAuth:ClientSecret"]
        };

        var tokenRequest = new AuthorizationCodeTokenRequest
        {
            Code = code,
            ClientId = clientSecrets.ClientId,
            ClientSecret = clientSecrets.ClientSecret,
            RedirectUri = _configuration["GoogleAuth:RedirectUri"],
        };

        var flow = new AuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = clientSecrets
        });

        var token = await flow.FetchTokenAsync("user", tokenRequest, CancellationToken.None);

        var userCredential = new UserCredential(flow, "user", token);

        // Save the credentials and call AddCredentials
        var userId = state; // Assuming you pass the user ID in the state parameter
        await _userServices.AddCredentials(userId, userCredential);

        // Redirect to a success page or send a response
        return Redirect($"http://localhost:3000/dashboard/user/{userId}/edit"); // Replace with your actual success URL
    }

    [HttpGet("google/callback/create-meeting")]
    public async Task<IActionResult> GoogleCallbackCreatMeeting(string code, string state)
    {
        if (string.IsNullOrEmpty(code))
        {
            return BadRequest("Authorization code is missing.");
        }

        var clientSecrets = new ClientSecrets
        {
            ClientId = _configuration["GoogleAuth:ClientId"],
            ClientSecret = _configuration["GoogleAuth:ClientSecret"]
        };

        var tokenRequest = new AuthorizationCodeTokenRequest
        {
            Code = code,
            ClientId = clientSecrets.ClientId,
            ClientSecret = clientSecrets.ClientSecret,
            RedirectUri = "http://localhost:7057/api/auth/google/callback/create-meeting",
        };

        var flow = new AuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = clientSecrets
        });

        var token = await flow.FetchTokenAsync("user", tokenRequest, CancellationToken.None);

        var userCredential = new UserCredential(flow, "user", token);

        // Save the credentials and call AddCredentials
        // Create MeetingFormCreating object
        JObject jsonObject = JObject.Parse(state);

        var meetingForm = new MeetingFormCreating
        {
            meeting_title = jsonObject["meeting_title"]?.ToString(),
            meeting_description = jsonObject["meeting_description"]?.ToString(),
            location = jsonObject["location"]?.ToString(),
            duration = Convert.ToInt32(jsonObject["duration"]),
            platform = Convert.ToInt32(jsonObject["platform"]),
            user_id = jsonObject["user_id"]?.ToString(),
            times = JsonConvert.DeserializeObject<List<DateTime>>(jsonObject["times"].ToString())
        };

        var userId = meetingForm.user_id; // Assuming you pass the user ID in the state parameter
        await _userServices.AddCredentials(userId, userCredential);
        await _meetingFormServices.CreateForm(meetingForm, userId);

        // Redirect to a success page or send a response
        return Redirect($"http://localhost:3000/dashboard"); // Replace with your actual success URL
    }
}
