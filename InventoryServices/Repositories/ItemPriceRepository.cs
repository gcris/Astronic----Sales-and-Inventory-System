using CommonLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using InventoryServices.ExtensionMethods;
using InventoryServices.Models;
using InventoryServices.Interfaces;

namespace InventoryServices.Repositories
{
    public class ItemPriceRepository : IItemPriceRepository
    {
        public async Task<bool> Save(ItemPriceDtos itemPriceDtos)
        {
            var dbContent = new InventoryDbContext();

            var itemPrice = itemPriceDtos.AsItemPrice();

            var item = await FindItem(itemPriceDtos.ItemId, dbContent);

            itemPrice.Item = item;

            if (item != null)
            {
                item.Price1 = itemPrice.Price1;

                item.Price2 = itemPrice.Price2;

                item.CurrentCost = itemPrice.CurrentCost;

                item.QuantityOnHand = itemPriceDtos.QuantityOnHand;

                dbContent.Entry(item).State = EntityState.Modified;
            }

            var queryItemPrice = await FindItemPrice(itemPrice.Id, dbContent);

            itemPrice.LastUpdate = DateTime.Now;

            if (queryItemPrice == null) dbContent.ItemPrices.Add(itemPrice);
            else
            {
                queryItemPrice.CurrentCost = itemPrice.CurrentCost;

                queryItemPrice.Item = itemPrice.Item;

                queryItemPrice.Price1 = itemPrice.Price1;

                queryItemPrice.Price2 = itemPrice.Price2;

                dbContent.Entry(queryItemPrice).State = EntityState.Modified;
            }

            return (await dbContent.SaveChangesAsync()) > 0;
        }

        public async Task<IEnumerable<ItemPriceDtos>> GetPriceListByItem(int id)
        {
            var dbContent = new InventoryDbContext();

            var queryPrices = await dbContent.ItemPrices
                .Include(item => item.Item)
                .Include(item => item.Item.Category)
                .Where(item => item.ItemId == id)
                .OrderByDescending(item => item.Item.PartNo)
                .ToListAsync();

            return queryPrices.Select(item => item.AsItemPriceDtos());
        }

        public async Task<IEnumerable<ItemPriceDtos>> GetPriceListByDate(int categoryId, DateTime date)
        {
            var dbContent = new InventoryDbContext();

            var queryPrices = await dbContent.ItemPrices
                .Include(item => item.Item)
                .Include(item => item.Item.Category)
                .Where(item => item.Item.CategoryId == categoryId)
                .ToListAsync();

            if (date > DateTime.MinValue) 
                queryPrices = queryPrices.Where(item => item.LastUpdate.Date == date.Date).ToList();

            return queryPrices
                .OrderByDescending(item => item.LastUpdate)
                .Select(item => item.AsItemPriceDtos());
        }

        #region Helper Methods
        private async Task<ItemPrice> FindItemPrice(int id, InventoryDbContext dbContent)
        {
            return await dbContent.ItemPrices
                .Include(item => item.Item)
                .FirstOrDefaultAsync(item => item.Id == id);
        }

        private async Task<Item> FindItem(int id, InventoryDbContext dbContent)
        {
            return await dbContent.Items
                .FirstOrDefaultAsync(item => item.Id == id);
        }
        #endregion
    }
}
