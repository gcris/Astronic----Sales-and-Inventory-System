using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Dtos
{
    public class PurchaseOrderDtos
    {
        public int PurchaseOrderId { get; set; }
        public DateTime Date { get; set; }
        public string PONumber { get; set; }
        public string Remarks { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal GrandTotalAmount { get; set; }
        public decimal TotalDiscount { get; set; }

        public string SupplierName { get; set; }
        public int SupplierId { get; set; }

        public IEnumerable<PurchaseOrderDetailDtos> PurchaseOrderDetailDtosList { get; set; }

        public bool IncludeDetails { get; set; }
        public bool Returned { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; }
    }
}
