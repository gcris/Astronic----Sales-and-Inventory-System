using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Dtos
{
    public class SalesSummaryDetailDtos
    {
        public string CategoryName { get; set; }
        public ItemDtos ItemDtos { get; set; }
        public string PartNo { get; set; }
        public string BrandName { get; set; }
        public string Model { get; set; }
        public string Make { get; set; }
        public string Made { get; set; }
        public string Size { get; set; }
        public decimal PurchasedAmount { get; set; }
        public decimal SoldAmount { get; set; }
        public decimal SalesReturnAmount { get; set; }
        public decimal PurchaseOrderReturnAmount { get; set; }
    }
}
