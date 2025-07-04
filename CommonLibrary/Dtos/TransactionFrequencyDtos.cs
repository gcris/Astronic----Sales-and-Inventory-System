using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Dtos
{
    public class TransactionFrequencyDtos
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Title { get; set; }
    }
}
