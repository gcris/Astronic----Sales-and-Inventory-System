using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Dtos
{
    public class InventorySummaryDetailDtos
    {
        public decimal BeginningInv { get; set; }
        public decimal QOH { get; set; }
        public decimal PurchasedItems { get; set; }
        public decimal SoldItems { get; set; }
        public decimal SalesReturnItems { get; set; }
        public decimal PurchaseOrderReturnItems { get; set; }
        public decimal EndingInv { get; set; }

        public int InventorySummaryId { get; set; }

        public int ItemId { get; set; }
        public ItemDtos ItemDtos { get; set; }

        public string PartNo { get; set; }
        public string BrandName { get; set; }
        public string Model { get; set; }
        public string Make { get; set; }
        public string Made { get; set; }
        public string Size { get; set; }
        public string CategoryName { get; set; }

        public DateTime Date { get; set; }
    }
}
