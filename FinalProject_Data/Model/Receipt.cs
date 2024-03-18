using System;
using System.Collections.Generic;
using System.Text;

namespace FinalProject_Data.Model
{
    public class Receipt
    {
        public int ID { get; set; }
        public int AccountID { get; set; }
        public int TotalSum { get; set; }
        public DateTime Date { get; set; }
        public Account Account { get; set; }
    }
}
