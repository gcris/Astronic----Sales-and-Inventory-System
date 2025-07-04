using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Dtos
{
    public class SalesIncomeDetailDtos
    {
        public decimal Price1 { get; set; }
        public decimal Price2 { get; set; }
        public decimal CurrentCost { get; set; }

        public string PartNo { get; set; }
        public string BrandName { get; set; }
        public string Model { get; set; }
        public string Make { get; set; }
        public string Made { get; set; }
        public string Size { get; set; }
        public string CategoryName { get; set; }
    }
}
