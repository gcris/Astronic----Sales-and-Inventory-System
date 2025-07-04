using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Dtos
{
    public class SalesIncomeDtos
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public decimal TotalPrice1 { get; set; }
        public decimal TotalPrice2 { get; set; }
        public decimal TotalCurrentCost { get; set; }

        public IEnumerable<SalesIncomeDetailDtos> SalesIncomeDetailDtosList { get; set; }
    }
}
