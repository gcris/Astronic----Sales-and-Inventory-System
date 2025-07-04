using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InventoryServices.Models
{
    public class PurchaseOrderReturnDetail
    {
        public int Id { get; set; }
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
        public decimal CurrentStock { get; set; }

        public virtual int PurchaseOrderDetailId { get; set; }
        public virtual PurchaseOrderDetail PurchaseOrderDetail { get; set; }

        public virtual int PurchaseOrderReturnId { get; set; }
        public virtual PurchaseOrderReturn PurchaseOrderReturn { get; set; }
    }
}
