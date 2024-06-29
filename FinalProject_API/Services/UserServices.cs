using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Hosting;
using FinalProject_Data;
using FinalProject_Data.Model;
using FinalProject_API.View.User;
using FinalProject_API.Common;
using FinalProject_Data.Enum;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FinalProject_API.View.Authentication;
using FinalProject_Data.Enum.Meeting;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using Microsoft.Identity.Client;
using FinalProject_API.View.GoogleCredentials;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Auth.OAuth2.Flows;
using Azure.Core;

namespace FinalProject_API.Services
{
    public interface IUserServices
    {
        Task<LoginResponse> Authenticate(LoginRequest request);
        Task<string> Create(UserCreating creating);
        Task<User> Get(string id, string actor_id);
        Task<List<User>> GetAll();
        Task<bool> Update(UserUpdating updating, string actor_id);
        Task<bool> RemoveCredentials(string user_id);
        //Task<bool> AddCredentials(string user_id);
        Task<bool> AddCredentials(string user_id, UserCredential credential);
    }

    public class UserServices : IUserServices
    {
        private readonly DatabaseContext _context;
        private readonly IAccountServices _accountService;
        private readonly IConfiguration _configuration;
        private readonly IMeetingFormServices _meetingFormServices;
        private readonly IMeetingServices _meetingServices;

        public UserServices(DatabaseContext context, IAccountServices accountService, IConfiguration configuration, IMeetingFormServices meetingFormServices, IMeetingServices meetingServices)
        {
            _context = context;
            _accountService = accountService;
            _configuration = configuration;
            _meetingFormServices = meetingFormServices;
            _meetingServices = meetingServices;
        }

        static string ApplicationName = "Scheduler";
        static string[] CalendarScopes = { CalendarService.Scope.Calendar };

        public async Task<LoginResponse> Authenticate(LoginRequest request)
        {
            var user = await GetByUserName(request.UserName);

            if (!Crypto.Verify(request.Password, user.salt, user.hash))
            {
                throw new InvalidProgramException("Wrong password");                
            }

            var access_token = RenderAccessToken(user);
            if (access_token != null)
            {
                return new LoginResponse
                {
                    id = user.ID,
                    name = user.name,
                    email = user.email,
                    access_token = access_token,
                    has_googlecredentials = user.has_googlecredentials,
                };
            }
            throw new Exception("Login failed");
        }

        public async Task<User> Get(string id, string actor_id)
        {
            var user = await _context.users
                .AsNoTracking()
                .FirstOrDefaultAsync(k => k.ID == id);
            if (user != null)
            {
                return user;
            }
            throw new InvalidProgramException($"Can't find user with id: '{id}'");
        }

        public async Task<List<User>> GetAll()
        {
            return await _context.users
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<string> Create(UserCreating creating)
        {
            try
            {
                if (creating.password != creating.confirm_password)
                {
                    throw new InvalidProgramException("Incorrect confirm password");
                }
                var user = new User
                {
                    ID = SlugID.New(),
                    name = creating.name,
                    username = creating.username,
                    email = creating.email
                };
                var password = creating.password;
                var userHash = _accountService.CreatePassword(password);
                user.hash = userHash.hash;
                user.salt = userHash.salt;

                _context.users.Add(user);
                if (await _context.SaveChangesAsync() > 0)
                {
                    return user.ID;
                }
                throw new Exception("Error saving informations");
            }
            catch (Exception ex)
            {
                if (ex.InnerException is SqlException)
                {
                    throw new InvalidProgramException(string.Format(((SqlNumber)((SqlException)ex.InnerException).Number).GetDescription(), GetFieldDuplicate(((SqlException)ex.InnerException).Message, creating)));
                }
                throw;
            }
        }

        public async Task<bool> Update(UserUpdating updating, string actor_id)
        {
            var user = await _context.users.AsNoTracking().FirstOrDefaultAsync(o => o.ID == updating.id);
            if (user != null)
            {
                user.name = updating.name;
                user.email = updating.email;

                _context.users.Update(user);
                return await _context.SaveChangesAsync() > 0;
            }
            throw new InvalidProgramException("Can't find user");
        }

        private string? GetFieldDuplicate(string exceptMessage, UserCreating creating)
        {
            if (exceptMessage.Contains($"users_{nameof(User.username)}"))
            {
                return $"Username '{creating.username}'";
            }
            if (exceptMessage.Contains($"users_{nameof(User.email)}"))
            {
                return $"Email '{creating.email}'";
            }
            if (exceptMessage.Contains($"users_{nameof(User.name)}"))
            {
                return $"Account name '{creating.name}'";
            }
            return null;
        }

        private string RenderAccessToken(User user, DateTime? expire = null)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(CustomJwtClaimType.Email, user.email),
                new Claim(CustomJwtClaimType.Name, user.name),
                new Claim(CustomJwtClaimType.Exp, DateTime.Now.AddDays(30).ToShortDateString()),
                new Claim(CustomJwtClaimType.UserId, user.ID)
            };

            string issuer = _configuration.GetSection("JwtSetting:Issuer").Value!;
            string audience = _configuration.GetSection("JwtSetting:Audience").Value!;
            string secretSercurityKey = _configuration.GetSection("JwtSetting:Token").Value!;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretSercurityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expire ?? DateTime.Now.AddDays(30), //DateTime.Now.AddDays(30),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<User> GetByUserName(string username)
        {
            var user = await _context.users.FirstOrDefaultAsync(o => o.username == username);
            if (user != null)
            {
                return user;
            }
            throw new InvalidProgramException("Can't find user");
        }

        public async Task<bool> RemoveCredentials(string user_id)
        {
            var user = await Get(user_id, user_id);
            var credentials = await _context.googlemeetcredentials.Where(o => o.user_id == user_id).ToListAsync();
            if (credentials != null)
            {
                _context.googlemeetcredentials.RemoveRange(credentials);

                var meeting_forms = await _meetingFormServices.GetAllForm(user_id);
                var meetings = await _meetingServices.GetAllMeeting(user_id);

                foreach( var meeting_form in meeting_forms)
                {
                    meeting_form.is_active = false;
                    _context.meetingforms.Update(meeting_form);
                }

                foreach (var meeting in meetings)
                {
                    meeting.is_active = false;
                    _context.meetings.Update(meeting);
                }

                user.has_googlecredentials = false;
                _context.users.Update(user);

                var client = new HttpClient();
                var revokeTokenEndpoint = $"https://oauth2.googleapis.com/revoke?token={credentials.OrderByDescending(o => o.IssuedUtc).FirstOrDefault().AccessToken}";

                var response = await client.PostAsync(revokeTokenEndpoint, null);
                response.EnsureSuccessStatusCode();

                return await _context.SaveChangesAsync() > 0;
            }
            throw new InvalidProgramException("Can't find user's credentials");
        }

        //public async Task<bool> AddCredentials(string user_id)
        //{
        //    var user = await Get(user_id, user_id);
        //    var credentials = await _context.googlemeetcredentials.FirstOrDefaultAsync(o => o.user_id == user_id);
        //    if (credentials != null)
        //    {
        //        throw new InvalidProgramException("User's already authenticated");
        //    }

        //    // Authenticate the user and save the token to the database
        //    UserCredential credential = await AuthenticateUserCalendarAsync();
        //    await SaveTokensToDatabase(credential, user_id);

        //    var meeting_forms = await _meetingFormServices.GetAllForm(user_id);
        //    var meetings = await _meetingServices.GetAllMeeting(user_id);

        //    foreach (var meeting_form in meeting_forms)
        //    {
        //        meeting_form.is_active = true;
        //        _context.meetingforms.Update(meeting_form);
        //    }

        //    foreach (var meeting in meetings)
        //    {
        //        meeting.is_active = true;
        //        _context.meetings.Update(meeting);
        //    }

        //    return await _context.SaveChangesAsync() > 0;
        //}

        public async Task<bool> AddCredentials(string user_id, UserCredential credential)
        {
            var user = await Get(user_id, user_id);
            var credentials = await _context.googlemeetcredentials.FirstOrDefaultAsync(o => o.user_id == user_id);
            if (credentials != null)
            {
                throw new InvalidProgramException("User's already authenticated");
            }

            // Authenticate the user and save the token to the database
            //UserCredential credential = await AuthenticateUserCalendarAsync();
            await SaveTokensToDatabase(credential, user_id);

            var meeting_forms = await _meetingFormServices.GetAllForm(user_id);
            var meetings = await _meetingServices.GetAllMeeting(user_id);

            foreach (var meeting_form in meeting_forms)
            {
                meeting_form.is_active = true;
                _context.meetingforms.Update(meeting_form);
            }

            foreach (var meeting in meetings)
            {
                meeting.is_active = true;
                _context.meetings.Update(meeting);
            }

            return await _context.SaveChangesAsync() > 0;
        }

        private async Task<UserCredential> AuthenticateUserCalendarAsync()
        {
            var dataStore = new FileDataStore(ApplicationName);

            // Clear existing credentials
            await dataStore.ClearAsync();

            return await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GetClientSecrets(),
                CalendarScopes,
                "user",
                CancellationToken.None,
                dataStore);
        }

        private async Task<bool> SaveTokensToDatabase(UserCredential credential, string actor_id)
        {
            var token = new GoogleMeetCredentials
            {
                ID = SlugID.New(),
                AccessToken = credential.Token.AccessToken,
                TokenType = credential.Token.TokenType,
                ExpiresIn = (int)credential.Token.ExpiresInSeconds,
                RefreshToken = credential.Token.RefreshToken,
                Scope = credential.Token.Scope,
                Issued = credential.Token.Issued,
                IssuedUtc = credential.Token.IssuedUtc,
                user_id = actor_id,
            };
            _context.googlemeetcredentials.Add(token);

            var user = await _context.users
                .AsNoTracking()
                .FirstOrDefaultAsync(k => k.ID == actor_id);
            if (user == null)
            {
                throw new InvalidProgramException($"Can't find user with id: '{actor_id}'");
            }
            user.has_googlecredentials = true;
            _context.users.Update(user);

            return await _context.SaveChangesAsync() > 0;
        }


        private ClientSecrets GetClientSecrets()
        {
            // Get the Google credentials from the configuration
            var googleCredentials = _configuration.GetSection("GoogleCredentials").Get<GoogleCredentials>();

            if (googleCredentials?.web == null)
            {
                throw new InvalidOperationException("Google credentials are not configured correctly in appsettings.json.");
            }

            // Create the client secrets object from the configuration
            var clientSecrets = new ClientSecrets
            {
                ClientId = googleCredentials.web.client_id,
                ClientSecret = googleCredentials.web.client_secret
            };

            return clientSecrets;
        }

    }
}
