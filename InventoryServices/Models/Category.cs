﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServices.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal MinimumStock { get; set; }
        public DateTime LastUpdate { get; set; }

        //public virtual ICollection<Item> Items { get; set; }
    }
}
