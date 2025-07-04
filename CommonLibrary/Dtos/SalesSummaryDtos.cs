using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Dtos
{
    public class SalesSummaryDtos
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public decimal TotalSold { get; set; }
        public decimal TotalSalesReturn { get; set; }
        public decimal TotalPurchase { get; set; }
        public decimal TotalPurchaseReturn { get; set; }

        public IEnumerable<SalesSummaryDetailDtos> SalesSummaryDetailDtosList { get; set; }
    }
}
