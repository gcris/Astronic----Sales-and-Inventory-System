using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Dtos
{
    public class UserActivityDtos
    {
        public int UserActivityId { get; set; }
        public string ReferenceNumber { get; set; }
        public DateTime Date { get; set; }
        public string Action { get; set; } // Transaction
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public decimal CurrentStock { get; set; }
        public string Remarks { get; set; }
        public bool Transaction { get; set; }
        public int ItemId { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; }
    }
}
