using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServices.Models
{
    public class ItemPrice
    {
        public int Id { get; set; }
        public decimal Price1 { get; set; }
        public decimal Price2 { get; set; }
        public decimal CurrentCost { get; set; }
        public DateTime LastUpdate { get; set; }

        public virtual int ItemId { get; set; }
        public virtual Item Item { get; set; }
    }
}
