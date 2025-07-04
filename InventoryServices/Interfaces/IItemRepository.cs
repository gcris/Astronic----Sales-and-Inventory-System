using CommonLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServices.Interfaces
{
    public interface IItemRepository
    {
        Task<int> Save(ItemDtos itemDtos);
        Task<ItemDtos> FindItemDtos(int id);
        Task<IEnumerable<ItemDtos>> GetAll(int categoryId, string key = "");
        Task<IEnumerable<ItemDtos>> GetAll();
        Task<bool> Delete(int id);

        Task<bool> SaveMinimumStocks(IEnumerable<ItemDtos> updatedItemDtosList);

        Task<IEnumerable<ItemTransactionDtos>> FindItemOnSalesInvoice(int itemId, DateTime from, DateTime to);
    }
}
