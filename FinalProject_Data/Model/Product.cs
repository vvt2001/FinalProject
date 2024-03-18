using System.Collections.Generic;

namespace FinalProject_Data.Model
{
    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int Category { get; set; }
        public List<Cart> Carts { get; set; }
    }
}