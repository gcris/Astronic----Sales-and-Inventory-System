using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InventoryServices.Models
{
    public class InventorySummaryDetail
    {
        public int Id { get; set; }

        public decimal BeginningInv { get; set; }
        public decimal PurchasedItems { get; set; }
        public decimal SoldItems { get; set; }
        public decimal SalesReturnItems { get; set; }
        public decimal PurchaseOrderReturnItems { get; set; }
        public decimal EndingInv { get; set; }

        public virtual int ItemId { get; set; }
        public virtual Item Item { get; set; }

        public virtual int InventorySummaryId { get; set; }
        public virtual InventorySummary InventorySummary { get; set; }
    }
}
