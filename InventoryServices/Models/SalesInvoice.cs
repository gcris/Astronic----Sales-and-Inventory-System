using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServices.Models
{
    public class SalesInvoice
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string ORNumber { get; set; }
        public string Remarks { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalDiscount { get; set; }

        public virtual int? CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public virtual ICollection<SalesInvoiceDetail> SalesInvoiceDetailList { get; set; }

        public bool Returned { get; set; }

        public virtual int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
