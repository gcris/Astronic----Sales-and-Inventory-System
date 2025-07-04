using InventoryServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using InventoryServices.Interfaces;
using CommonLibrary.Dtos;
using InventoryServices.ExtensionMethods;

namespace InventoryServices.Repositories
{
    public class ItemRepository : IItemRepository
    {
        public async Task<int> Save(ItemDtos itemDtos)
        {
            var dbContext = new InventoryDbContext();

            var item = itemDtos.AsItem();

            var queryItem = await FindItem(item.Id, dbContext);

            var category = await FindCategory(itemDtos.CategoryId, dbContext);

            item.Category = category;

            item.Supplier = await FindSupplier(itemDtos.SupplierId, dbContext);

            if (item.SupplierId.HasValue)
            {
                if (item.SupplierId.Value > 0)
                    item.Supplier = await FindSupplier(itemDtos.SupplierId, dbContext);
                else item.SupplierId = null;
            }

            item.LastUpdate = DateTime.Now;

            if (itemDtos.ItemPriceDtosList != null)
            {
                var itemPriceDtos = itemDtos.ItemPriceDtosList.FirstOrDefault();

                var itemPrice = itemPriceDtos.AsItemPrice();

                itemPrice.LastUpdate = DateTime.Now;

                itemPrice.Item = queryItem == null ? item : queryItem;

                var queryItemPrice = await FindItemPrice(itemPriceDtos.ItemPriceId, dbContext);

                if (queryItemPrice == null) dbContext.ItemPrices.Add(itemPrice);
                else
                {
                    queryItemPrice.Item = itemPrice.Item;

                    queryItemPrice.Price1 = itemPrice.Price1;

                    queryItemPrice.Price2 = itemPrice.Price2;

                    queryItemPrice.CurrentCost = itemPrice.CurrentCost;

                    dbContext.Entry(queryItemPrice).State = EntityState.Modified;
                }
            }

            if (queryItem == null) dbContext.Items.Add(item);
            else
            {
                queryItem.Category = item.Category;

                queryItem.Supplier = item.Supplier;

                queryItem.BrandName = item.BrandName;

                queryItem.Made = item.Made;

                queryItem.Make = item.Make;

                queryItem.Model = item.Model;

                queryItem.PartNo = item.PartNo;

                queryItem.QuantityOnHand = item.QuantityOnHand;

                queryItem.OtherPartNo = item.OtherPartNo;

                queryItem.MinimumStock = item.MinimumStock;

                queryItem.UnitOfMeasure = item.UnitOfMeasure;

                queryItem.Size = item.Size;

                queryItem.Price1 = item.Price1;

                queryItem.Price2 = item.Price2;

                queryItem.CurrentCost = item.CurrentCost;

                queryItem.Supplier = item.Supplier;

                dbContext.Entry(queryItem).State = EntityState.Modified;
            }

            await dbContext.SaveChangesAsync();

            return queryItem == null ? item.Id : queryItem.Id;
        }

        public async Task<bool> SaveMinimumStocks(IEnumerable<ItemDtos> itemDtosList)
        {
            var dbContext = new InventoryDbContext();

            foreach (var itemDtos in itemDtosList)
            {
                var item = itemDtos.AsItem();

                var queryItem = await FindItem(item.Id, dbContext);

                queryItem.MinimumStock = itemDtos.MinimumStock;

                dbContext.Entry(queryItem).State = EntityState.Modified;
            }

            return (await dbContext.SaveChangesAsync()) > 0;
        }

        public async Task<ItemDtos> FindItemDtos(int id)
        {
            var dbContext = new InventoryDbContext();

            var queryItem = await FindItem(id, dbContext);

            if (queryItem == null) return null;

            return queryItem.AsItemDtos();
        }

        public async Task<IEnumerable<ItemDtos>> GetAll(int categoryId, string key = "")
        {
            key = key.ToLower();
            using (InventoryDbContext dbContext = new InventoryDbContext())
            {

                var queryItemList = await dbContext.Items
                .Include(item => item.Category)
                .Include(item => item.Supplier)
                .Where(item => categoryId == 0 ? true : item.CategoryId == categoryId)
                .Where(item => 
                    (item.Category.Name.ToLower().Contains(key) ||
                    item.BrandName.ToLower().Contains(key) ||
                    item.Made.ToLower().Contains(key) ||
                    item.Make.ToLower().Contains(key) ||
                    item.Model.ToLower().Contains(key) ||
                    item.PartNo.ToLower().Contains(key) ||
                    item.OtherPartNo.ToLower().Contains(key) ||
                    item.Size.ToLower().Contains(key)))
                .OrderBy(item => item.PartNo)
                .ToListAsync();

                return queryItemList.Select(item => item.AsItemDtos());
            }
        }

        public async Task<IEnumerable<ItemDtos>> GetAll()
        {
            using (InventoryDbContext dbContext = new InventoryDbContext())
            {
                var queryItemList = await dbContext.Items
                    .Include(item => item.Category)
                    .Include(item => item.Supplier)
                    .OrderByDescending(item => item.LastUpdate)
                    .ToListAsync();

                return queryItemList.AsParallel().Select(item => item.AsItemDtos());
            }

        
        }

        public async Task<bool> Delete(int id)
        {
            var dbContext = new InventoryDbContext();

            var query = await FindItem(id, dbContext);

            //var exists = await dbContext..AnyAsync(detail => detail.ItemId == query.Id);

            if (query != null) dbContext.Items.Remove(query);

            return (await dbContext.SaveChangesAsync()) > 0;
        }

        public async Task<IEnumerable<ItemTransactionDtos>> FindItemOnSalesInvoice(int itemId, DateTime from, DateTime to)
        {
            var dbContext = new InventoryDbContext();

            var salesInvoiceDetails = await dbContext.SalesInvoiceDetails
                .Include(detail => detail.Item)
                .Include(detail => detail.Item.Supplier)
                .Include(detail => detail.Item.Category)
                .Include(detail => detail.SalesInvoice)
                .Include(detail => detail.SalesInvoice.User)
                .Where(detail => detail.ItemId == itemId && !detail.SalesInvoice.Returned)
                .ToListAsync();

            if (from > DateTime.MinValue && to > DateTime.MinValue)
                salesInvoiceDetails = salesInvoiceDetails.Where(detail =>
                    detail.SalesInvoice.Date.Date >= from.Date &&
                    detail.SalesInvoice.Date.Date <= to.Date).ToList();

            var salesReturnDetails = await dbContext.SalesReturnDetails
                .Include(detail => detail.SalesInvoiceDetail.Item)
                .Include(detail => detail.SalesInvoiceDetail.Item.Supplier)
                .Include(detail => detail.SalesInvoiceDetail.Item.Category)
                .Include(detail => detail.SalesReturn)
                .Include(detail => detail.SalesReturn.User)
                .Where(detail => detail.SalesInvoiceDetail.ItemId == itemId)
                .ToListAsync();

            if (from > DateTime.MinValue && to > DateTime.MinValue)
                salesReturnDetails = salesReturnDetails.Where(detail =>
                    detail.SalesReturn.Date.Date >= from.Date &&
                    detail.SalesReturn.Date.Date <= to.Date).ToList();

            var purchaseOrderDetails = await dbContext.PurchaseOrderDetails
                .Include(detail => detail.Item)
                .Include(detail => detail.Item.Supplier)
                .Include(detail => detail.Item.Category)
                .Include(detail => detail.PurchaseOrder)
                .Include(detail => detail.PurchaseOrder.User)
                .Where(detail => detail.ItemId == itemId && !detail.PurchaseOrder.Returned)
                .ToListAsync();

            if (from > DateTime.MinValue && to > DateTime.MinValue)
                purchaseOrderDetails = purchaseOrderDetails.Where(detail =>
                    detail.PurchaseOrder.Date.Date >= from.Date &&
                    detail.PurchaseOrder.Date.Date <= to.Date).ToList();

            var poReturnDetails = await dbContext.PurchaseOrderReturnDetails
               .Include(detail => detail.PurchaseOrderDetail.Item)
               .Include(detail => detail.PurchaseOrderDetail.Item.Supplier)
               .Include(detail => detail.PurchaseOrderDetail.Item.Category)
               .Include(detail => detail.PurchaseOrderReturn)
               .Include(detail => detail.PurchaseOrderReturn.User)
               .Where(detail => detail.PurchaseOrderDetail.ItemId == itemId)
               .ToListAsync();

            if (from > DateTime.MinValue && to > DateTime.MinValue)
                poReturnDetails = poReturnDetails.Where(detail =>
                    detail.PurchaseOrderReturn.Date.Date >= from.Date &&
                    detail.PurchaseOrderReturn.Date.Date <= to.Date).ToList();

            var itemTransactions = new List<ItemTransactionDtos>();

            var salesTask = Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(salesInvoiceDetails.GroupBy(item => item.SalesInvoiceId), details =>
                {
                    var name = !string.IsNullOrWhiteSpace(details.FirstOrDefault().SalesInvoice.User.LastName) ?
                        string.Format("{0}, {1}", details.FirstOrDefault().SalesInvoice.User.LastName,
                        details.FirstOrDefault().SalesInvoice.User.FirstName) : details.FirstOrDefault().SalesInvoice.User.Username;

                    itemTransactions.Add(new ItemTransactionDtos
                    {
                        Amount = details.Sum(detail => detail.TotalAmount),
                        CurrentStock = details.FirstOrDefault().CurrentStock,
                        Quantity = details.Sum(detail => detail.Quantity),
                        Date = details.FirstOrDefault().SalesInvoice.Date,
                        ItemDtos = details.FirstOrDefault().Item.AsItemDtos(),
                        ReferenceNumber = details.FirstOrDefault().SalesInvoice.ORNumber,
                        TransactionType = "Sales Invoice (OUT)",
                        Remarks = details.FirstOrDefault().SalesInvoice.Remarks,
                        EncodeBy = name
                    });
                });

                Parallel.ForEach(salesReturnDetails.GroupBy(item => item.SalesReturnId), details =>
                {
                    var name = string.Format("{0}, {1}",
                        details.FirstOrDefault().SalesReturn.User.LastName,
                        details.FirstOrDefault().SalesReturn.User.FirstName);

                    itemTransactions.Add(new ItemTransactionDtos
                    {
                        Amount = details.Sum(detail => detail.Amount),
                        CurrentStock = details.FirstOrDefault().CurrentStock,
                        Quantity = details.Sum(detail => detail.Quantity),
                        Date = details.FirstOrDefault().SalesReturn.Date,
                        ItemDtos = details.FirstOrDefault().SalesInvoiceDetail.Item.AsItemDtos(),
                        ReferenceNumber = details.FirstOrDefault().SalesReturn.ReferenceNumber,
                        TransactionType = "Sales Return (IN-RETURNED)",
                        Remarks = details.FirstOrDefault().Remarks,
                        EncodeBy = name
                    });
                });

                Parallel.ForEach(purchaseOrderDetails.GroupBy(item => item.PurchaseOrderId), details =>
                {
                    var name = string.Format("{0}, {1}",
                        details.FirstOrDefault().PurchaseOrder.User.LastName,
                        details.FirstOrDefault().PurchaseOrder.User.FirstName);

                    itemTransactions.Add(new ItemTransactionDtos
                    {
                        Amount = details.Sum(detail => detail.TotalAmount),
                        CurrentStock = details.FirstOrDefault().CurrentStock,
                        Quantity = details.Sum(detail => detail.Quantity),
                        Date = details.FirstOrDefault().PurchaseOrder.Date,
                        ItemDtos = details.FirstOrDefault().Item.AsItemDtos(),
                        ReferenceNumber = details.FirstOrDefault().PurchaseOrder.PONumber,
                        TransactionType = "Purchase Order (IN)",
                        Remarks = details.FirstOrDefault().PurchaseOrder.Remarks,
                        EncodeBy = name
                    });
                });

                Parallel.ForEach(poReturnDetails.GroupBy(item => item.PurchaseOrderReturnId), details =>
                {
                    var name = string.Format("{0}, {1}",
                        details.FirstOrDefault().PurchaseOrderReturn.User.LastName,
                        details.FirstOrDefault().PurchaseOrderReturn.User.FirstName);

                    itemTransactions.Add(new ItemTransactionDtos
                    {
                        Amount = details.Sum(detail => detail.Amount),
                        CurrentStock = details.FirstOrDefault().CurrentStock,
                        Quantity = details.Sum(detail => detail.Quantity),
                        Date = details.FirstOrDefault().PurchaseOrderReturn.Date,
                        ItemDtos = details.FirstOrDefault().PurchaseOrderDetail.Item.AsItemDtos(),
                        ReferenceNumber = details.FirstOrDefault().PurchaseOrderReturn.ReferenceNumber,
                        TransactionType = "Purchase Order Return (OUT-RETURNED)",
                        Remarks = details.FirstOrDefault().Remarks,
                        EncodeBy = name
                    });
                });
            });

            salesTask.Wait();

            return itemTransactions.OrderByDescending(item => item.Date);
        }

        #region Helper Method
        private async Task<Item> FindItem(int id, InventoryDbContext dbContext)
        {
            return await dbContext.Items
                .Include(item => item.Category)
                .Include(item => item.Supplier)
                .FirstOrDefaultAsync(item => item.Id == id);
        }

        private async Task<ItemPrice> FindItemPrice(int id, InventoryDbContext dbContext)
        {
            return await dbContext.ItemPrices
                .Include(item => item.Item)
                .Include(item => item.Item.Supplier)
                .FirstOrDefaultAsync(item => item.Id == id);
        }

        private async Task<Category> FindCategory(int id, InventoryDbContext dbContext)
        {
            return await dbContext.Categories
                .FirstOrDefaultAsync(cat => cat.Id == id);
        }

        private async Task<Supplier> FindSupplier(int id, InventoryDbContext dbContext)
        {
            return await dbContext.Suppliers
                .FirstOrDefaultAsync(supplier => supplier.Id == id);
        }

        private async Task<Category> FindCategory(string name, InventoryDbContext dbContext)
        {
            return await dbContext.Categories
                .FirstOrDefaultAsync(cat => cat.Name == name);
        }
        #endregion
    }
}
