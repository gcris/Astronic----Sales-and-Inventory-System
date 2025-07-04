using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLibrary.Enums;

namespace InventoryServices.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string PartNo { get; set; }
        public string OtherPartNo { get; set; }
        public string BrandName { get; set; }
        public string Model { get; set; }
        public string Make { get; set; }
        public string Made { get; set; }
        public string Size { get; set; }
        public decimal QuantityOnHand { get; set; }
        public decimal MinimumStock { get; set; }
        public DateTime LastUpdate { get; set; }

        public string UnitOfMeasure { get; set; }

        public virtual int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public virtual int? SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }

        public decimal Price1 { get; set; }
        public decimal Price2 { get; set; }
        public decimal CurrentCost { get; set; }
    }
}
