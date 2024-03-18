using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject_Data.Model
{
    public class Cart
    {
        public int ID { get; set; }
        public int AccountID { get; set; }
        public int ProductID { get; set; }
        public int ProductQuantity { get; set; }
        public int TotalPrice { get; set; }
        public Account Account { get; set; }
        public Product Product { get; set; }
    }
}
