using CommonLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServices.Interfaces
{
    public interface IInventoryRepository
    {
        Task<DateTime> GetLastInventoryDate();
        Task<bool> Save(InventorySummaryDtos inventorySummaryDtos);
        Task<IEnumerable<InventorySummaryDtos>> GetInventory(DateTime from, DateTime to);
    }
}
