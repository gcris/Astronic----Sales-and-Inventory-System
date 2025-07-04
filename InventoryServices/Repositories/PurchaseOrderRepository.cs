using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using InventoryServices.ExtensionMethods;
using CommonLibrary.Dtos;
using InventoryServices.Models;
using InventoryServices.Interfaces;

namespace InventoryServices.Repositories
{
    public class PurchaseOrderRepository : IPurchaseOrderRepository
    {
        public async Task<bool> Save(PurchaseOrderDtos purchaseOrderDtos, List<int> deletedIdList)
        {
            var dbContext = new InventoryDbContext();

            // Delete purchase order details
            if (deletedIdList != null) await Delete(deletedIdList, dbContext);

            // Save purchase order
            var purchaseOrder = purchaseOrderDtos.AsPurchaseOrder();

            purchaseOrder.User = await FindUser(purchaseOrder.UserId, dbContext);

            purchaseOrder.Supplier = await FindSupplier(purchaseOrder.SupplierId, dbContext);

            foreach (var detailDtos in purchaseOrderDtos.PurchaseOrderDetailDtosList)
            {
                var detail = detailDtos.AsPurchaseOrderDetail();

                await UpdateQuantityOnHand(detailDtos.ItemDtos.ItemId, 0, detail.Quantity, detail.UnitPrice, detailDtos.SellingPrice, purchaseOrderDtos.SupplierId, purchaseOrder.Date, dbContext);

                detail.Item = await FindItem(detailDtos.ItemDtos.ItemId, dbContext);

                detail.CurrentStock = detail.Item.QuantityOnHand;

                detail.PurchaseOrder = purchaseOrder;

                dbContext.PurchaseOrderDetails.Add(detail);
            }

            dbContext.PurchaseOrders.Add(purchaseOrder);

            return (await dbContext.SaveChangesAsync()) > 0;
        }

        public async Task<PurchaseOrderDtos> FindPurchaseOrderDtos(int id)
        {
            var dbContext = new InventoryDbContext();

            var query = await FindPurchaseOrder(id, dbContext);

            if (query == null) return null;

            return query.AsPurchaseOrderDtos();
        }

        public async Task<PurchaseOrderDtos> FindPurchaseOrderDtos(string poNumber)
        {
            var dbContext = new InventoryDbContext();

            var query = await FindPurchaseOrder(poNumber, dbContext);

            if (query == null) return null;

            return query.AsPurchaseOrderDtos();
        }

        public async Task<PurchaseOrderDetailDtos> FindPurchaseOrderDetailDtos(int id)
        {
            var dbContext = new InventoryDbContext();

            var query = await FindPurchaseOrderDetail(id, dbContext);

            if (query == null) return null;

            return query.AsPurchaseOrderDetailDtos();
        }

        public async Task<IEnumerable<PurchaseOrderDtos>> GetAll(string key = "")
        {
            var dbContext = new InventoryDbContext();

            int currentYear = DateTime.Now.Year;

                DateTime dateFrom = DateTime.Now.AddMonths(-1);
                DateTime dateTo = DateTime.Now.Date;

                var query = await dbContext.PurchaseOrders
                    .Include(order => order.User)
                    .Include(order => order.PurchaseOrderDetailList)
                    .Include(order => order.PurchaseOrderDetailList.Select(detail => detail.Item))
                    .Include(order => order.PurchaseOrderDetailList.Select(detail => detail.Item.Category))
                    .Where(order => order.Date >= dateFrom && order.Date <= dateTo && (order.PONumber.Contains(key) ||
                        order.Supplier.Company.Contains(key)) && !order.Returned)
                    .OrderByDescending(order => order.Date)
                    .ToListAsync();

                return query.Select(order => order.AsPurchaseOrderDtos());
            

        }
        public async Task<IEnumerable<PurchaseOrderDtos>> GetByDateRange(DateTime from, DateTime to, string key = "")
        {
            var dbContext = new InventoryDbContext();

            var query = await dbContext.PurchaseOrders
                    .Include(order => order.User)
                    .Include(order => order.PurchaseOrderDetailList)
                    .Include(order => order.PurchaseOrderDetailList.Select(detail => detail.Item))
                    .Include(order => order.PurchaseOrderDetailList.Select(detail => detail.Item.Category))
                    .Where(order => (order.Date >= from && order.Date <= to) && (order.PONumber.Contains(key) ||
                        order.Supplier.Company.Contains(key)) && !order.Returned)
                    .OrderByDescending(order => order.Date)
                    .ToListAsync();

                return query.Select(order => order.AsPurchaseOrderDtos());
            
        }

        public async Task<bool> Delete(int id)
        {
            var dbContext = new InventoryDbContext();

            var query = await FindPurchaseOrder(id, dbContext);

            dbContext.PurchaseOrders.Remove(query);

            return (await dbContext.SaveChangesAsync()) > 0;
        }

        #region Helper Method
        private async Task<User> FindUser(int id, InventoryDbContext dbContext)
        {
            return await dbContext.Users.FirstOrDefaultAsync(user => user.Id == id);
        }

        private async Task Delete(List<int> idList, InventoryDbContext dbContext)
        {
            foreach (var id in idList)
            {
                var detail = await FindPurchaseOrderDetail(id, dbContext);

                if (detail != null)
                {
                    await UpdateQuantityOnHand(detail.Item.Id, detail.Quantity, 0, 0, 0, 0, DateTime.Now, dbContext);

                    dbContext.PurchaseOrderDetails.Remove(detail);
                }
            }
        }

        private async Task<PurchaseOrder> FindPurchaseOrder(int id, InventoryDbContext dbContext)
        {
            return await dbContext.PurchaseOrders
                .Include(order => order.User)
                .Include(order => order.PurchaseOrderDetailList)
                .Include(order => order.PurchaseOrderDetailList.Select(detail => detail.Item))
                .Include(order => order.PurchaseOrderDetailList.Select(detail => detail.Item.Category))
                .FirstOrDefaultAsync(order => order.Id == id);
        }

        private async Task<PurchaseOrder> FindPurchaseOrder(string poNumber, InventoryDbContext dbContext)
        {
            return await dbContext.PurchaseOrders
                .Include(order => order.User)
                .Include(order => order.PurchaseOrderDetailList)
                .Include(order => order.PurchaseOrderDetailList.Select(detail => detail.Item))
                .Include(order => order.PurchaseOrderDetailList.Select(detail => detail.Item.Category))
                .FirstOrDefaultAsync(order => order.PONumber == poNumber);
        }

        private async Task<PurchaseOrderDetail> FindPurchaseOrderDetail(int id, InventoryDbContext dbContext)
        {
            return await dbContext.PurchaseOrderDetails
                .Include(order => order.Item)
                .Include(order => order.Item.Category)
                .Include(order => order.PurchaseOrder)
                .Include(order => order.PurchaseOrder.Supplier)
                .FirstOrDefaultAsync(order => order.Id == id);
        }

        private async Task<Item> FindItem(int itemId, InventoryDbContext dbContext)
        {
            return await dbContext.Items
                .Include(order => order.Category)
                .FirstOrDefaultAsync(item => item.Id == itemId);
        }

        private async Task<Supplier> FindSupplier(int id, InventoryDbContext dbContext)
        {
            return await dbContext.Suppliers.FirstOrDefaultAsync(item => item.Id == id);
        }

        private async Task UpdateQuantityOnHand(int itemId, decimal oldQty, decimal newQty, decimal currentCost, decimal sellingPrice, int supplierId, DateTime date, InventoryDbContext dbContext)
        {
            var queryItem = await FindItem(itemId, dbContext);

            if (queryItem != null)
            {
                queryItem.QuantityOnHand -= oldQty;

                queryItem.QuantityOnHand += newQty;

                queryItem.LastUpdate = date;

                if (currentCost > 0) queryItem.CurrentCost = currentCost;

                if (sellingPrice > 0) queryItem.Price1 = sellingPrice;

                if (supplierId > 0) queryItem.Supplier = await FindSupplier(supplierId, dbContext);

                dbContext.Entry(queryItem).State = EntityState.Modified;
            }
        }
        #endregion
    }
}
