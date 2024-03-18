using System;
using System.Collections.Generic;
using System.Text;

namespace FinalProject_Data.Model
{
    public class ProductSearching
    {
        public string SearchString { get; set; }
        public int LowerPrice { get; set; }
        public int UpperPrice { get; set; }
        public int Category { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
