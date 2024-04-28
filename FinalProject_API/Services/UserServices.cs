﻿using AutoMapper;
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
    public interface IUserServices
    {
        Task<LoginResponse> Authenticate(LoginRequest request);
        Task<string> Create(UserCreating creating);
        Task<User> Get(string id, string actor_id);
        Task<List<User>> GetAll();
    }

    public class UserServices : IUserServices
    {
        private readonly DatabaseContext _context;
        private readonly IAccountServices _accountService;
        private readonly IConfiguration _configuration;

        public UserServices(DatabaseContext context, IAccountServices accountService, IConfiguration configuration)
        {
            _context = context;
            _accountService = accountService;
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

        public async Task<string> Create(UserCreating creating)
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
            throw new InvalidProgramException("Người dùng không tồn tại");
        }
    }
}