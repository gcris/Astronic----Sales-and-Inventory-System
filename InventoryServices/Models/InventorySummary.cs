using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServices.Models
{
    public class InventorySummary
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public decimal TotalSold { get; set; }
        public decimal TotalSalesReturn { get; set; }
        public decimal TotalPurchase { get; set; }
        public decimal TotalPurchaseReturn { get; set; }

        public virtual ICollection<InventorySummaryDetail> InventorySummaryDetailList { get; set; }
    }
}
