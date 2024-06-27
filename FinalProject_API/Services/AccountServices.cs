using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinalProject_API.Common;
using FinalProject_Data;
using FinalProject_Data.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FinalProject_API.Services
{
    public interface IAccountServices
    {
        Task<User> Get(string user_id);
        Task<bool> ChangePassword(string current_password, string new_password, string actor_id);
        bool VerifyPassword(User user, string password);
        UserHash CreatePassword(string passwordText);
        bool ValidatePassword(string passwordText);
    }
    public class AccountServices : IAccountServices
    {
        private readonly DatabaseContext _context;
        public AccountServices(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<bool> ChangePassword(string current_password, string new_password, string actor_id)
        {
            var account = await Get(actor_id);

            if (VerifyPassword(account, current_password))
            {
                var userinfo = CreatePassword(new_password);

                account.salt = userinfo.salt;
                account.hash = userinfo.hash;

                _context.users.Update(account);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<User> Get(string user_id)
        {
            var user = await _context.users.AsNoTracking().FirstOrDefaultAsync(o => o.ID == user_id);
            if (user != null)
            {
                return user;
            }
            throw new InvalidProgramException("Can't find user");
        }

        public bool VerifyPassword(User user, string password)
        {
            /* valid password */
            if (!Crypto.Verify(password, user.salt, user.hash))
            {
                throw new InvalidProgramException("Wrong password");
            }

            return true;
        }

        public UserHash CreatePassword(string passwordText)
        {
            /*** validate password ***/
            if (!ValidatePassword(passwordText))
            {
                throw new InvalidProgramException("Password invalid");
            }
            /*************************/
            var result = new UserHash();
            result.salt = Crypto.CreateSalt();
            result.hash = Crypto.Hash(passwordText, result.salt);

            return result;
        }

        public bool ValidatePassword(string passwordText)
        {
            var validator = new PasswordValidator
            {
                //MinLength = 8,
                //RequireDigit = true,
                //RequireLowercase = true,
                //RequireNonLetterOrDigit = true,
                //RequireUppercase = true

                MinLength = 0,
                RequireDigit = false,
                RequireLowercase = false,
                RequireNonLetterOrDigit = false,
                RequireUppercase = false
            };
            return validator.Validate(passwordText);
        }
    }
}
