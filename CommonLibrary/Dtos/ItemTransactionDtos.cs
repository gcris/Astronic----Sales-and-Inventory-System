using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Dtos
{
    public class ItemTransactionDtos
    {
        public ItemDtos ItemDtos { get; set; }
        public string ReferenceNumber { get; set; }
        public DateTime Date { get; set; }
        public string TransactionType { get; set; }
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public decimal CurrentStock { get; set; }
        public string Remarks { get; set; }
        public string EncodeBy { get; set; }
    }
}
