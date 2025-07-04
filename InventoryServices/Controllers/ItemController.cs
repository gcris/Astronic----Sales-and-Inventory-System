using CommonLibrary.Dtos;
using CommonLibrary.Enums;
using InventoryServices.Interfaces;
using InventoryServices.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using InventoryServices.ExtensionMethods;

namespace InventoryServices.Controllers
{
	public class ItemController
	{
		private IItemRepository itemRepository = new ItemRepository();
		private SalesInvoiceController salesInvoiceController = new SalesInvoiceController();
		private SalesReturnController salesReturnController = new SalesReturnController();
		private PurchaseOrderController purchaseOrderController = new PurchaseOrderController();
		private PurchaseOrderReturnController purchaseOrderReturnController = new PurchaseOrderReturnController();
		//private ISalesInvoiceRepository salesInvoiceRepository = new SalesInvoiceRepository();
		//private IPurchaseOrderRepository purchaseOrderRepository = new PurchaseOrderRepository();

		public async Task<int> Save(ItemDtos itemDtos)
		{
			return await itemRepository.Save(itemDtos);
		}

		//public async Task<IEnumerable<InventorySummaryDtos>> ProcessInventorySummary(DateTime from, DateTime to, string key = "", int categoryId = 0)
		//{
		//    var items = await itemRepository.GetAll(key);

		//    if (categoryId > 0) items = items.Where(item => item.CategoryId == categoryId);

		//    var salesInvoices = await salesInvoiceRepository.GetAll(key);

		//    salesInvoices = salesInvoices.Where(item => item.DateRequested.Date >= from.Date && item.DateRequested.Date <= to.Date);

		//    if (categoryId > 0) salesInvoices = salesInvoices.Where(item => item.SalesInvoiceDetailDtosList.Any(detail => detail.CategoryId == categoryId));

		//    var purchases = await purchaseOrderRepository.GetAll(key);

		//    purchases = purchases.Where(item => item.Date.Date >= from.Date && item.Date.Date <= to.Date);

		//    if (categoryId > 0) purchases = purchases.Where(item => item.PurchaseOrderDetailDtosList.Any(detail => detail.CategoryId == categoryId));

		//    var invDtosList = new List<InventorySummaryDtos>();

		//    foreach (var item in items)
		//    {
		//        var invSummary = new InventorySummaryDtos
		//        {
		//            CategoryName = item.CategoryName,
		//            ParentCategoryName = item.ParentCategoryName,
		//            ItemCode = item.Code,
		//            ItemName = item.Name,
		//            ItemSize = item.Size,
		//            QOH = item.QuantityOnHand
		//        };

		//        foreach (var purchase in purchases)
		//        {
		//            var exists = purchase.PurchaseOrderDetailDtosList.Any(p => p.ItemId == item.ItemId);

		//            if (exists)
		//            {
		//                invSummary.PurchasedItems += purchase.PurchaseOrderDetailDtosList
		//                    .Where(p => p.ItemId == item.ItemId).Sum(p => p.Quantity);
		//            }
		//        }

		//        foreach (var sales in salesInvoices)
		//        {
		//            var exists = sales.SalesInvoiceDetailDtosList.Any(p => p.ItemId == item.ItemId);

		//            if (exists)
		//            {
		//                invSummary.SoldItems += sales.SalesInvoiceDetailDtosList
		//                    .Where(p => p.ItemId == item.ItemId).Sum(p => p.Quantity);
		//            }
		//        }

		//        var lastPurchase = purchases.OrderByDescending(p => p.Date).FirstOrDefault();

		//        if (lastPurchase != null)
		//        {
		//            var exists = lastPurchase.PurchaseOrderDetailDtosList.Any(p => p.ItemId == item.ItemId);

		//            if (exists)
		//            {
		//                var detail = lastPurchase.PurchaseOrderDetailDtosList.FirstOrDefault(p => p.ItemId == item.ItemId);

		//                if (detail != null)
		//                {
		//                    invSummary.UnitPrice = detail.UnitPrice;
		//                }
		//            }
		//        }

		//        invDtosList.Add(invSummary);
		//    }

		//    return invDtosList;
		//}

		public async Task<IEnumerable<ItemTransactionDtos>> FindTransaction(int itemId, DateTime from, DateTime to)
		{
			return await itemRepository.FindItemOnSalesInvoice(itemId, from, to);
		}

		public async Task<ItemDtos> Find(int id)
		{
			return await itemRepository.FindItemDtos(id);
		}

		public async Task<IEnumerable<ItemDtos>> GetAll(int categoryId, string key = "")
		{
			return await itemRepository.GetAll(categoryId, key);
		}

        public async Task<IEnumerable<ItemDtos>> GetAll()
        {
            return await itemRepository.GetAll();
        }

		public async Task<IEnumerable<ItemDtos>> GetMinimumStock(bool withQuantity = true)
		{
            var queryList = await itemRepository.GetAll();

			return queryList.Where(item => item.MinimumStock > item.QuantityOnHand &&
				(withQuantity ? item.QuantityOnHand > 0 : true));
		}

        public async Task<IEnumerable<ItemDtos>> GetNonMovingItems(int categoryId, string key = "", bool withQuantity = true)
		{
			var queryList = await itemRepository.GetAll(categoryId, key);

			return queryList.Where(item => item.LastUpdate.Year <= DateTime.Now.AddYears(-1).Year &&
				(withQuantity ? item.QuantityOnHand > 0 : true));
		}

        public async Task<IEnumerable<ItemDtos>> GetMovingItems(int categoryId, string key = "", bool slowMoving = true, bool withQuantity = true)
		{
			var queryList = await itemRepository.GetAll(categoryId, key);

			var list = new List<ItemDtos>();

			var salesInvoiceList = await salesInvoiceController.GetAll(null, DateTime.MinValue, DateTime.MinValue);

			var salesInvoiceDetailList = new List<SalesInvoiceDetailDtos>();

			var movingMonth = slowMoving ? DateTime.Now.AddMonths(-1) : DateTime.Now;

			foreach (var salesInvoice in salesInvoiceList.Where(item => item.Date.Month >= movingMonth.Month))
			{
				salesInvoiceDetailList.AddRange(salesInvoice.SalesInvoiceDetailDtosList.Where(detail => detail != null));
			}

			foreach (var salesDetails in salesInvoiceDetailList.Where(detail => detail != null).GroupBy(detail => detail.ItemDtos.ItemId))
			{
				if (salesDetails.Count() > 0)
				{
					var percentSales = salesDetails.FirstOrDefault().ItemDtos.QuantityOnHand * 0.20m;

					var flag = slowMoving ? salesDetails.Sum(detail => detail.Quantity) <= percentSales :
						salesDetails.Sum(detail => detail.Quantity) > percentSales;

					if (flag) list.Add(salesDetails.FirstOrDefault().ItemDtos);
				}
			}

			return list.OrderBy(item => item.PartNo);
		}

		public async Task<IEnumerable<ItemDtos>> GetAllByCategoryAndSupplier(IEnumerable<ItemDtos> itemDtosList, string key = "", int categoryId = 0, int supplierId = 0)
		{
            IEnumerable<ItemDtos> queryItemDtosList = null;

            if (categoryId < 1) queryItemDtosList = await itemRepository.GetAll();
			else queryItemDtosList = await itemRepository.GetAll(categoryId, key);
			if (supplierId > 0) queryItemDtosList = queryItemDtosList.Where(cat => cat.SupplierId == supplierId);
			return queryItemDtosList;
		}

		public async Task<bool> IsValidPartNo(int categoryId, string oldPartNo, string newPartNo, string model, string size, string categoryName)
		{
            var queryDtosList = await itemRepository.GetAll(categoryId);

			var valid = true;

			if (queryDtosList.Count() > 0)
			{
				queryDtosList = queryDtosList.Where(item =>
					item.CategoryName == categoryName && 
					item.Model == model &&
					item.Size == size);

				if (!string.IsNullOrWhiteSpace(oldPartNo))
					queryDtosList = queryDtosList.Where(item => item.PartNo != oldPartNo);

				valid = !queryDtosList.Any(item => item.PartNo == newPartNo);
			}

			return valid;
		}

		public async Task<bool> IsValidModel(int categoryId, string oldModel, string newModel, string partNo, string categoryName)
		{
            var queryDtosList = await itemRepository.GetAll(categoryId);

			var valid = true;

			if (queryDtosList.Count() > 0)
			{
				queryDtosList = queryDtosList.Where(item =>
					item.CategoryName == categoryName &&
					item.PartNo == partNo);

				if (!string.IsNullOrWhiteSpace(oldModel))
					queryDtosList = queryDtosList.Where(item => item.Model != oldModel);

				valid = !queryDtosList.Any(item => item.Model == newModel);
			}

			return valid;
		}

		//public async Task<IEnumerable<ItemDtos>> FastMovingItems()
		//{
		//    var queryDtosList = await itemRepository.GetAll();

		//    var salesInvoiceDtosList = await salesInvoiceRepository.GetAll();

		//    var topSalesItems = new List<ItemDtos>();

		//    foreach (var salesInvoice in salesInvoiceDtosList)
		//    {
		//        var detailDtosList = salesInvoice.SalesInvoiceDetailDtosList
		//            .GroupBy(detail => detail.ItemId)
		//            .OrderByDescending(detail => detail.Sum(d => d.Quantity)).Take(5);

		//        foreach (var detail in detailDtosList)
		//        {
		//            var itemDtos = await itemRepository.FindItemDtos(detail.FirstOrDefault().ItemId);

		//            itemDtos.TotalSold = detail.Sum(d => d.Quantity);

		//            topSalesItems.Add(itemDtos);
		//        }
		//    }

		//    return topSalesItems.OrderByDescending(item => item.TotalSold);
		//}

		//public async Task<IEnumerable<ItemDtos>> SlowMovingItems()
		//{
		//    var queryDtosList = await itemRepository.GetAll();

		//    var salesInvoiceDtosList = await salesInvoiceRepository.GetAll();

		//    var topSalesItems = new List<ItemDtos>();

		//    foreach (var salesInvoice in salesInvoiceDtosList)
		//    {
		//        var detailDtosList = salesInvoice.SalesInvoiceDetailDtosList
		//            .GroupBy(detail => detail.ItemId)
		//            .OrderBy(detail => detail.Sum(d => d.Quantity));

		//        foreach (var detail in detailDtosList.Where(detail => detail.Sum(d => d.Quantity) <= 10))
		//        {
		//            var itemDtos = await itemRepository.FindItemDtos(detail.FirstOrDefault().ItemId);

		//            itemDtos.TotalSold = detail.Sum(d => d.Quantity);

		//            topSalesItems.Add(itemDtos);
		//        }
		//    }

		//    return topSalesItems.OrderByDescending(item => item.TotalSold);
		//}

		//public async Task<IEnumerable<ItemDtos>> MinimumStockItems()
		//{
		//    var queryDtosList = await itemRepository.GetAll();

		//    return queryDtosList.Where(item => item.QuantityOnHand <= 10);
		//}

		public async Task<bool> Delete(int id)
		{
			return await itemRepository.Delete(id);
		}

		public IEnumerable<string> GetUnitOfMeasureList()
		{
			var uomList = Enum.GetNames(typeof(UnitOfMeasure));

			return uomList.ToList();
		}

		public async Task<bool> SaveMinimumStocks(List<ItemDtos> updatedItemDtosList)
		{
			return await itemRepository.SaveMinimumStocks(updatedItemDtosList.Where(item => item.MinimumStock > 0));
		}
	}
}
