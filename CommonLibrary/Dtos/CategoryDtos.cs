using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Dtos
{
    public class CategoryDtos
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public decimal MinimumStock { get; set; }
        public DateTime LastUpdate { get; set; }

        //public IEnumerable<ItemDtos> ItemDtosList { get; set; }
    }
}
