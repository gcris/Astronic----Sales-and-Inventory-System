using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServices.Models
{
    public class SalesReturnDetail
    {
        public int Id { get; set; }
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
        public decimal CurrentStock { get; set; }

        public virtual int SalesInvoiceDetailId { get; set; }
        public virtual SalesInvoiceDetail SalesInvoiceDetail { get; set; }

        public virtual int SalesReturnId { get; set; }
        public virtual SalesReturn SalesReturn { get; set; }
    }
}
