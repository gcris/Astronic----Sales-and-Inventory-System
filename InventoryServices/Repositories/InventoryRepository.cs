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
    public class InventoryRepository : IInventoryRepository
    {
        public async Task<DateTime> GetLastInventoryDate()
        {
            var dbContext = new InventoryDbContext();

            var list = await dbContext.InventorySummaries
                .OrderByDescending(item => item.Date)
                .ToListAsync();

            return list.Count > 0 ? list.FirstOrDefault().Date : new DateTime(2018, 12, 4);
        }

        public async Task<bool> Save(InventorySummaryDtos inventorySummaryDtos)
        {
            var dbContext = new InventoryDbContext();

            var inventorySummary = inventorySummaryDtos.AsInventorySummary();

            var queryInventorySummary = await FindInventorySummary(inventorySummary.Date, dbContext);

            //var detailDtosList = inventorySummaryDtos.InventorySummaryDetailDtosList;

            //foreach (var detailDtos in detailDtosList)
            //{
            //    var detail = detailDtos.AsInventorySummaryDetail();

            //    detail.Item = await FindItem(detail.ItemId, dbContext);

            //    detail.InventorySummary = queryInventorySummary == null ? inventorySummary : queryInventorySummary;

            //    var queryDetail = await FindInventorySummaryDetail(detail.Id, dbContext);

            //    if (queryDetail == null) dbContext.InventorySummaryDetails.Add(detail);
            //    else
            //    {
            //        queryDetail.PurchasedItems = detail.PurchasedItems;

            //        queryDetail.PurchaseOrderReturnItems = detail.PurchaseOrderReturnItems;

            //        queryDetail.SalesReturnItems = detail.SalesReturnItems;

            //        queryDetail.SoldItems = detail.SoldItems;

            //        queryDetail.BeginningInv = detail.BeginningInv;

            //        queryDetail.EndingInv = detail.EndingInv;

            //        dbContext.Entry(queryDetail).State = EntityState.Modified;
            //    }
            //}

            if (queryInventorySummary == null) dbContext.InventorySummaries.Add(inventorySummary);
            else
            {
                queryInventorySummary.TotalPurchase = inventorySummary.TotalPurchase;

                queryInventorySummary.TotalPurchaseReturn = inventorySummary.TotalPurchaseReturn;

                queryInventorySummary.TotalSalesReturn = inventorySummary.TotalSalesReturn;

                queryInventorySummary.TotalSold = inventorySummary.TotalSold;

                dbContext.Entry(queryInventorySummary).State = EntityState.Modified;
            }

            return (await dbContext.SaveChangesAsync()) > 0;
        }

        public async Task<IEnumerable<InventorySummaryDtos>> GetInventory(DateTime from, DateTime to)
        {
            var dbContext = new InventoryDbContext();

            var list = await dbContext.InventorySummaries
                .Include(item => item.InventorySummaryDetailList)
                .Include(item => item.InventorySummaryDetailList.Select(detail => detail.Item))
                .Include(item => item.InventorySummaryDetailList.Select(detail => detail.Item.Supplier))
                .Include(item => item.InventorySummaryDetailList.Select(detail => detail.Item.Category))           
                .OrderBy(item => item.Date)
                .ToListAsync();

            list = list.Where(item => item.Date.Date >= from.Date.Date && item.Date <= to.Date).ToList();

            return list.Select(item => item.AsInventorySummaryDtos());
        } 

        #region Helper Method
        private async Task<InventorySummary> FindInventorySummary(DateTime date, InventoryDbContext dbContext)
        {
            return await dbContext.InventorySummaries
                .Include(item => item.InventorySummaryDetailList)
                .Include(item => item.InventorySummaryDetailList.Select(detail => detail.Item))
                .Include(item => item.InventorySummaryDetailList.Select(detail => detail.Item.Supplier))
                .Include(item => item.InventorySummaryDetailList.Select(detail => detail.Item.Category))
                .FirstOrDefaultAsync(item => item.Date == date.Date);
        }

        private async Task<InventorySummaryDetail> FindInventorySummaryDetail(int id, InventoryDbContext dbContext)
        {
            return await dbContext.InventorySummaryDetails
                .Include(item => item.Item)
                .Include(item => item.Item.Supplier)
                .Include(item => item.Item.Category)
                .Include(item => item.InventorySummary)
                .FirstOrDefaultAsync(item => item.Id == id);
        }

        private async Task<Item> FindItem(int id, InventoryDbContext dbContext)
        {
            return await dbContext.Items
                .Include(item => item.Supplier)
                .Include(item => item.Category)
                .FirstOrDefaultAsync(item => item.Id == id);
        }
        #endregion
    }
}
