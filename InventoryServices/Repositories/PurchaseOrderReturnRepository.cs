using CommonLibrary.Dtos;
using InventoryServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using InventoryServices.ExtensionMethods;
using InventoryServices.Interfaces;

namespace InventoryServices.Repositories
{
    public class PurchaseOrderReturnRepository : IPurchaseOrderReturnRepository
    {
        public async Task<bool> Save(PurchaseOrderReturnDtos purchaseOrderReturnDtos)
        {
            var dbContext = new InventoryDbContext();

            var purchaseOrderReturn = purchaseOrderReturnDtos.AsPurchaseOrderReturn();

            purchaseOrderReturn.User = await FindUser(purchaseOrderReturn.UserId, dbContext);

            foreach (var poReturnDetailDtos in purchaseOrderReturnDtos.PurchaseOrderReturnDetailDtosList)
            {
                var poReturnDetail = poReturnDetailDtos.AsPurchaseOrderReturnDetail();

                await UpdateQuantityOnHand(
                    poReturnDetailDtos.ItemDtos.ItemId,
                    0,
                    poReturnDetailDtos.Quantity,
                    purchaseOrderReturn.Date,
                    dbContext);

                var poDetail = await FindPurchaseOrderDetail(poReturnDetailDtos.PurchaseOrderDetailId, dbContext);

                if (poDetail != null)
                {
                    poDetail.Quantity -= poReturnDetail.Quantity;

                    poDetail.TotalAmount = poDetail.Quantity * poDetail.UnitPrice;

                    dbContext.Entry(poDetail).State = EntityState.Modified;

                    if (poDetail.PurchaseOrder != null)
                    {
                        var purchaseOrder = await FindPurchaseOrder(poDetail.PurchaseOrderId, dbContext);

                        purchaseOrder.GrandTotalAmount = purchaseOrder.PurchaseOrderDetailList.Sum(detail => detail.TotalAmount);

                        purchaseOrder.TotalQuantity = purchaseOrder.PurchaseOrderDetailList.Sum(detail => detail.Quantity);

                        purchaseOrder.Returned = purchaseOrder.GrandTotalAmount == 0 && purchaseOrder.TotalQuantity == 0;

                        dbContext.Entry(purchaseOrder).State = EntityState.Modified;
                    }
                }

                poReturnDetail.PurchaseOrderDetail = poDetail;

                poReturnDetail.PurchaseOrderReturn = purchaseOrderReturn;

                poReturnDetail.CurrentStock = poReturnDetail.PurchaseOrderDetail.Item.QuantityOnHand;

                dbContext.PurchaseOrderReturnDetails.Add(poReturnDetail);
            }

            dbContext.PurchaseOrderReturns.Add(purchaseOrderReturn);

            return (await dbContext.SaveChangesAsync()) > 0;
        }

        public async Task<IEnumerable<PurchaseOrderReturnDtos>> GetAll(string key = "")
        {
            using (InventoryDbContext dbContext = new InventoryDbContext())
            {
                int currentYear = DateTime.Now.Year;
                DateTime dateFrom = DateTime.Now.AddMonths(-1);
                DateTime dateTo = DateTime.Now.Date;

                var queryList = await dbContext.PurchaseOrderReturns
                    .Include(item => item.User)
                    .Include(item => item.PurchaseOrderReturnDetailList)
                    .Include(item => item.PurchaseOrderReturnDetailList.Select(detail => detail.PurchaseOrderDetail.Item))
                    //.Include(item => item.PurchaseOrderReturnDetailList.Select(detail => detail.PurchaseOrderDetail.PurchaseOrder))
                    .Where(item => (item.Date >= dateFrom && item.Date <= dateTo) &&  item.ReferenceNumber.Contains(key))
                    .OrderByDescending(item => item.Date)
                    .ToListAsync();

                return queryList.Select(item => item.AsPurchaseOrderReturnDtos());
            }
        }
        public async Task<IEnumerable<PurchaseOrderReturnDtos>> GetByDateRange(DateTime from, DateTime to, string key = "")
        {
            using (InventoryDbContext dbContext = new InventoryDbContext())
            {
                var queryList = await dbContext.PurchaseOrderReturns
                    .Include(item => item.User)
                    .Include(item => item.PurchaseOrderReturnDetailList)
                    .Include(item => item.PurchaseOrderReturnDetailList.Select(detail => detail.PurchaseOrderDetail.Item))
                    //.Include(item => item.PurchaseOrderReturnDetailList.Select(detail => detail.PurchaseOrderDetail.PurchaseOrder))
                    .Where(item => (item.Date >= from && item.Date <= to) && item.ReferenceNumber.Contains(key))
                    .OrderByDescending(item => item.Date)
                    .ToListAsync();

                return queryList.Select(item => item.AsPurchaseOrderReturnDtos());
            }
        }

        public async Task<PurchaseOrderReturnDtos> FindPurchaseOrderReturn(int id)
        {
            var dbContext = new InventoryDbContext();

            var querySalesReturn = await FindPurchaseOrderReturn(id, dbContext);

            if (querySalesReturn == null) return null;

            return querySalesReturn.AsPurchaseOrderReturnDtos();
        }

        public async Task<PurchaseOrderReturnDtos> FindPurchaseOrderReturn(string referenceNumber)
        {
            var dbContext = new InventoryDbContext();

            var querySalesReturn = await FindPurchaseOrderReturn(referenceNumber, dbContext);

            if (querySalesReturn == null) return null;

            return querySalesReturn.AsPurchaseOrderReturnDtos();
        }

        public async Task<bool> DeleteDetail(int id)
        {
            var dbContext = new InventoryDbContext();

            var queryDetail = await FindPurchaseOrderReturnDetail(id, dbContext);

            if (queryDetail != null)
            {
                var poReturnId = queryDetail.PurchaseOrderDetailId;

                await UpdateQuantityOnHand(queryDetail.PurchaseOrderDetail.ItemId,
                    queryDetail.Quantity, 
                    0,
                    DateTime.Now,
                    dbContext);

                var poDetail = await FindPurchaseOrderDetail(queryDetail.PurchaseOrderDetailId, dbContext);

                if (poDetail != null)
                {
                    poDetail.Quantity += queryDetail.Quantity;

                    poDetail.TotalAmount = poDetail.Quantity * poDetail.UnitPrice;

                    dbContext.Entry(poDetail).State = EntityState.Modified;

                    var purchaseOrder = await FindPurchaseOrder(poDetail.PurchaseOrderId, dbContext);

                    if (purchaseOrder != null)
                    {
                        purchaseOrder.TotalQuantity = purchaseOrder.PurchaseOrderDetailList.Sum(detail => detail.Quantity);

                        purchaseOrder.GrandTotalAmount = purchaseOrder.PurchaseOrderDetailList.Sum(detail => detail.TotalAmount);

                        purchaseOrder.Returned = purchaseOrder.GrandTotalAmount == 0 && purchaseOrder.TotalQuantity == 0;

                        dbContext.Entry(purchaseOrder).State = EntityState.Modified;
                    }
                }

                dbContext.PurchaseOrderReturnDetails.Remove(queryDetail);

                var poReturn = await FindPurchaseOrderReturn(poReturnId, dbContext);

                if (poReturn != null)
                {
                    if (poReturn.PurchaseOrderReturnDetailList == null)
                    {
                        dbContext.PurchaseOrderReturns.Remove(poReturn);
                    }
                }
            }

            return (await dbContext.SaveChangesAsync()) > 0;
        }

        public async Task<bool> Delete(int id)
        {
            var dbContext = new InventoryDbContext();

            var queryPoReturn = await FindPurchaseOrderReturn(id, dbContext);

            if (queryPoReturn != null)
            {
                foreach (var queryDetail in queryPoReturn.PurchaseOrderReturnDetailList)
                {
                    await UpdateQuantityOnHand(queryDetail.PurchaseOrderDetail.ItemId,
                        queryDetail.Quantity, 
                        0,
                        DateTime.Now,
                        dbContext);

                    var poDetail = await FindPurchaseOrderDetail(queryDetail.PurchaseOrderDetailId, dbContext);

                    if (poDetail != null)
                    {
                        poDetail.Quantity += queryDetail.Quantity;

                        poDetail.TotalAmount = poDetail.Quantity * poDetail.UnitPrice;

                        dbContext.Entry(poDetail).State = EntityState.Modified;

                        var purchaseOrder = await FindPurchaseOrder(poDetail.PurchaseOrderId, dbContext);

                        if (purchaseOrder != null)
                        {
                            purchaseOrder.TotalQuantity = purchaseOrder.PurchaseOrderDetailList.Sum(detail => detail.Quantity);

                            purchaseOrder.GrandTotalAmount = purchaseOrder.PurchaseOrderDetailList.Sum(detail => detail.TotalAmount);

                            purchaseOrder.Returned = purchaseOrder.GrandTotalAmount == 0 && purchaseOrder.TotalQuantity == 0;

                            dbContext.Entry(purchaseOrder).State = EntityState.Modified;
                        }
                    }
                }

                dbContext.PurchaseOrderReturns.Remove(queryPoReturn);
            }

            return (await dbContext.SaveChangesAsync()) > 0;
        }

        private async Task<PurchaseOrderReturn> FindPurchaseOrderReturn(int id, InventoryDbContext dbContext)
        {
            return await dbContext.PurchaseOrderReturns
                .Include(item => item.User)
                .Include(item => item.PurchaseOrderReturnDetailList)
                .Include(item => item.PurchaseOrderReturnDetailList.Select(detail => detail.PurchaseOrderDetail))
                .Include(item => item.PurchaseOrderReturnDetailList.Select(detail => detail.PurchaseOrderDetail.Item))
                .FirstOrDefaultAsync(item => item.Id == id);
        }

        private async Task<PurchaseOrderReturn> FindPurchaseOrderReturn(string referenceNumber, InventoryDbContext dbContext)
        {
            return await dbContext.PurchaseOrderReturns
                .Include(item => item.User)
                .Include(item => item.PurchaseOrderReturnDetailList)
                .Include(item => item.PurchaseOrderReturnDetailList.Select(detail => detail.PurchaseOrderDetail))
                .Include(item => item.PurchaseOrderReturnDetailList.Select(detail => detail.PurchaseOrderDetail.Item))
                .FirstOrDefaultAsync(item => item.ReferenceNumber == referenceNumber);
        }

        #region Helper
        private async Task<User> FindUser(int id, InventoryDbContext dbContext)
        {
            return await dbContext.Users.FirstOrDefaultAsync(user => user.Id == id);
        }

        private async Task<PurchaseOrderReturnDetail> FindPurchaseOrderReturnDetail(int id, InventoryDbContext dbContext)
        {
            return await dbContext.PurchaseOrderReturnDetails
                .Include(item => item.PurchaseOrderDetail)
                .Include(item => item.PurchaseOrderDetail.PurchaseOrder)
                .Include(item => item.PurchaseOrderDetail.Item)
                .FirstOrDefaultAsync(item => item.Id == id);
        }

        private async Task<PurchaseOrderDetail> FindPurchaseOrderDetail(int id, InventoryDbContext dbContext)
        {
            return await dbContext.PurchaseOrderDetails.FirstOrDefaultAsync(item => item.Id == id);
        }

        private async Task<PurchaseOrder> FindPurchaseOrder(int id, InventoryDbContext dbContext)
        {
            return await dbContext.PurchaseOrders.FirstOrDefaultAsync(item => item.Id == id);
        }

        private async Task<Item> FindItem(int id, InventoryDbContext dbContext)
        {
            return await dbContext.Items
                .Include(item => item.Category)
                .FirstOrDefaultAsync(item => item.Id == id);
        }

        private async Task UpdateQuantityOnHand(int id, decimal oldQty, decimal newQty, DateTime date, InventoryDbContext dbContext)
        {
            var queryItem = await FindItem(id, dbContext);

            if (queryItem != null)
            {
                queryItem.QuantityOnHand += oldQty;

                queryItem.QuantityOnHand -= newQty;

                queryItem.LastUpdate = date;

                dbContext.Entry(queryItem).State = EntityState.Modified;
            }
        }
        #endregion
    }
}
