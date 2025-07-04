using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Dtos
{
    public class SalesInvoiceDtos
    {
        public int SalesInvoiceId { get; set; }
        public DateTime Date { get; set; }
        public string ORNumber { get; set; }
        public string Remarks { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalDiscount { get; set; }

        public int CustomerId { get; set; }
        public string CustomerName { get; set; }

        public IEnumerable<SalesInvoiceDetailDtos> SalesInvoiceDetailDtosList { get; set; }
        public bool Returned { get; set; }

        public bool IncludeDetails { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; }
    }
}
