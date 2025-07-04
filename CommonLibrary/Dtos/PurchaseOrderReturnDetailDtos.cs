using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Dtos
{
    public class PurchaseOrderReturnDetailDtos
    {
        public int PurchaseOrderReturnDetailId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
        public decimal CurrentStock { get; set; }

        public ItemDtos ItemDtos { get; set; }

        public int PurchaseOrderDetailId { get; set; }

        public int PurchaseOrderId { get; set; }
    }
}
