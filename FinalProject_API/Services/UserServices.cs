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

namespace FinalProject_API.Services
{
    public interface IUserService
    {
        Task<LoginResponse> Authenticate(LoginRequest request);
        Task<string> Create(UserCreating nguoiDungCreating, string actor_id);
        Task<User> Get(string id, string actor_id);
        Task<List<User>> GetAll();
    }

    public class UserService : IUserService
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;
        private readonly IAccountServices _accountService;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public UserService(DatabaseContext context, IMapper mapper, IAccountServices accountService, IWebHostEnvironment env, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _accountService = accountService;
            _env = env;
            _configuration = configuration;
        }



        public async Task<LoginResponse> Authenticate(LoginRequest request)
        {
            var user = await GetByUserName(request.UserName);

            if (!Crypto.Verify(request.Password, user.salt, user.hash))
            {
                throw new InvalidProgramException($"Mật khẩu không chính xác.");                
            }

            var access_token = RenderAccessToken(user);
            if (access_token != null)
            {
                return new LoginResponse
                {
                    id = user.ID,
                    name = user.name,
                    email = user.email,
                    access_token = access_token
                };
            }
            throw new Exception("Đăng nhập không thành công");
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
            throw new InvalidProgramException($"Không tìm thấy Người dùng với id '{id}'");
        }

        public async Task<List<User>> GetAll()
        {
            return await _context.users
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<string> Create(UserCreating creating, string actor_id)
        {
            try
            {
                var user = new User
                {
                    ID = SlugID.New(),
                    name = creating.name,
                    username = creating.user_name,
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
                throw new Exception("Xảy ra lỗi trong quá trình lưu thông tin");
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

        private string? GetFieldDuplicate(string exceptMessage, UserCreating creating)
        {
            if (exceptMessage.Contains($"{nameof(User)}_{nameof(User.username)}"))
            {
                return $"Tên đăng nhập '{creating.user_name}'";
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

            string issuer = _configuration.GetValue<string>("TokenAuthentication:Issuer");
            string secretSercurityKey = _configuration.GetValue<string>("TokenAuthentication:SecretSercurityKey");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretSercurityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(issuer,
                issuer,
                claims,
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
            throw new InvalidProgramException("Người dùng không tồn tại");
        }
    }
}
