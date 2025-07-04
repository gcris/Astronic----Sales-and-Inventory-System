using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Dtos
{
    public class ItemPriceDtos
    {
        public int ItemPriceId { get; set; }
        public decimal QuantityOnHand { get; set; }
        public decimal Price1 { get; set; }
        public decimal Price2 { get; set; }
        public decimal CurrentCost { get; set; }
        public DateTime LastUpdate { get; set; }

        public int ItemId { get; set; }
        public string ItemPartNo { get; set; }

        public string CategoryName { get; set; }
    }
}
