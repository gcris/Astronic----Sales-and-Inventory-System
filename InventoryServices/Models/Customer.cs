using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InventoryServices.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string ContactNo { get; set; }
    }
}
