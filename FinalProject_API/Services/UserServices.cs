using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Hosting;
using FinalProject_Data;
using FinalProject_Data.Model;
using FinalProject_API.View.User;
using FinalProject_API.Common;
using FinalProject_Data.Enum;

namespace FinalProject_API.Services
{
    public interface IUserService
    {
        Task<string> Create(UserCreating nguoiDungCreating, string actor_id);
        Task<User> Get(string id, string actor_id);
        Task<List<User>> GetAll();
    }

    public class UserService : IUserService
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IAccountServices _accountService;
        private readonly IWebHostEnvironment _env;

        public UserService(DatabaseContext context, IMapper mapper, IUserService userService, IAccountServices accountService, IWebHostEnvironment env)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
            _accountService = accountService;
            _env = env;
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
    }
}
