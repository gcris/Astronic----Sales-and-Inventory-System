using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServices.Models
{
    public class PurchaseOrder
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string PONumber { get; set; }
        public string Remarks { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal GrandTotalAmount { get; set; }
        public decimal TotalDiscount { get; set; }

        public virtual int SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }

        public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetailList { get; set; }

        public bool Returned { get; set; }

        public virtual int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
