using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinalProject_Data;
using FinalProject_Data.Model;
using Microsoft.EntityFrameworkCore;

namespace FinalProject_API.Services
{
    public interface IAccountServices
    {
        Account Create(Account Account);
        void Delete(int id);
        Account Get(int id);
        Account Edit(int id, string password, string phonenumber, string address);
    }
    public class AccountServices : IAccountServices
    {
        private readonly DatabaseContext _context;
        public AccountServices(DatabaseContext context)
        {
            _context = context;
        }
        public Account Create(Account Account)
        {
            _context.Add(Account);
            _context.SaveChanges();
            return Account;
        }

        public void Delete(int id)
        {
            var Account = _context.Accounts.Where(x => x.ID == id).Include(x=>x.Carts).FirstOrDefault();
            _context.Accounts.Remove(Account);
            _context.SaveChanges();
        }

        public Account Edit(int id, string password, string phonenumber, string address)
        {
            var account = _context.Accounts.Find(id);
            account.Password = password;
            account.Phonenumber = phonenumber;
            account.Address = address;
            _context.Entry(account).State = EntityState.Modified;
            _context.SaveChanges();
            return account;
        }

        public Account Get(int id)
        {
            var Account = _context.Accounts.Where(Account => Account.ID == id).FirstOrDefault();
            return Account;
        }
    }
}
