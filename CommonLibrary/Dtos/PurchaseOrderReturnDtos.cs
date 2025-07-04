using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Dtos
{
    public class PurchaseOrderReturnDtos
    {
        public int PurchaseOrderReturnId { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public string ReferenceNumber { get; set; }

        public IEnumerable<PurchaseOrderReturnDetailDtos> PurchaseOrderReturnDetailDtosList { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; }
    }
}
