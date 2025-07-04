using CommonLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServices.Interfaces
{
    public interface IItemPriceRepository
    {
        Task<bool> Save(ItemPriceDtos itemPriceDtos);
        Task<IEnumerable<ItemPriceDtos>> GetPriceListByItem(int id);

        Task<IEnumerable<ItemPriceDtos>> GetPriceListByDate(int categoryId, DateTime date);
    }
}
