using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Dtos
{
    public class SalesInvoiceDetailDtos
    {
        public int SalesInvoiceDetailId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal CurrentStock { get; set; }

        public decimal Price2 { get; set; }
        public decimal CurrentCost { get; set; }

        public ItemDtos ItemDtos { get; set; }
        public string PartNo { get; set; }
        public string BrandName { get; set; }
        public string Model { get; set; }
        public string Make { get; set; }
        public string Made { get; set; }
        public string Size { get; set; }
        public string CategoryName { get; set; }

        public int SalesInvoiceId { get; set; }
    }
}
