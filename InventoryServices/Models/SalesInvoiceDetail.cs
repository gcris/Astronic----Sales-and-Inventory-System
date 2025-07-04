using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InventoryServices.Models
{
    public class SalesInvoiceDetail
    {
        public int Id { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal CurrentStock { get; set; }

        public decimal Price2 { get; set; }
        public decimal CurrentCost { get; set; }

        public virtual int ItemId { get; set; }
        public virtual Item Item { get; set; }

        public virtual int SalesInvoiceId { get; set; }
        public virtual SalesInvoice SalesInvoice { get; set; }
    }
}
