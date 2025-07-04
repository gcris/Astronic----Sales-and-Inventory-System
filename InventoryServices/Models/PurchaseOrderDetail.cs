using CommonLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InventoryServices.Models
{
    public class PurchaseOrderDetail
    {
        public int Id { get; set; }
        public decimal Quantity { get; set; }
        public string UOM { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal CurrentStock { get; set; }

        public virtual int ItemId { get; set; }
        public virtual Item Item { get; set; }

        public virtual int PurchaseOrderId { get; set; }
        public virtual PurchaseOrder PurchaseOrder { get; set; }
    }
}
