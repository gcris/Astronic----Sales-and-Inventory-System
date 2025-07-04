using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InventoryServices.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        public string ContactPerson { get; set; }
        public string Company { get; set; }
        public string Address { get; set; }
        public string ContactNo { get; set; }
    }
}
