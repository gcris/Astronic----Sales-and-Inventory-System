using CommonLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Dtos
{
    public class ItemDtos
    {
        public int ItemId { get; set; }
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
        public string UnitOfMeasureString { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public int SupplierId { get; set; }
        public string SupplierName { get; set; }

        public IEnumerable<ItemPriceDtos> ItemPriceDtosList { get; set; }

        public decimal Price1 { get; set; }
        public decimal Price2 { get; set; }
        public decimal CurrentCost { get; set; }
    }
}
