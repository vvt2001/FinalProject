using System;
using System.Collections.Generic;
using System.Text;

namespace FinalProject_Data.Model
{
    public class Account
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Phonenumber { get; set; }
        public string Address { get; set; }
        public List<Cart> Carts { get; set; }
        public List<Receipt> Receipts { get; set; }

}
}
