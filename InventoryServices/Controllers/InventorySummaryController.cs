using CommonLibrary.Dtos;
using InventoryServices.Interfaces;
using InventoryServices.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServices.Controllers
{
    public class InventorySummaryController
    {
        private SalesInvoiceController salesInvoiceController = new SalesInvoiceController();
        private SalesReturnController salesReturnController = new SalesReturnController();
        private PurchaseOrderController purchaseOrderController = new PurchaseOrderController();
        private PurchaseOrderReturnController purchaseOrderReturnController = new PurchaseOrderReturnController();
        private IInventoryRepository invRepository = new InventoryRepository();
        private IItemRepository itemRepository = new ItemRepository();

        //private IEnumerable<SalesInvoiceDtos> salesInvoiceDtosList;
        //private IEnumerable<SalesReturnDtos> salesReturnDtosList;
        //private IEnumerable<PurchaseOrderDtos> purchaseOrderDtosList;
        //private IEnumerable<PurchaseOrderReturnDtos> purchaseOrderReturnDtosList;
        private IEnumerable<ItemDtos> itemDtosList;

        private DateTime from, to;

        public async Task<IEnumerable<InventorySummaryDtos>> GetDailyInventory(DateTime from, DateTime to, int categoryId = 0)
        {
            var inventorySummaryDtosList = await ProcessInventorySummary(from, to, "", categoryId);

            return inventorySummaryDtosList;
        }

        public async Task<IEnumerable<InventorySummaryDtos>> GetInventory(DateTime from, DateTime to, int categoryId = 0)
        {
            //var inventorySummaryDtosList = await invRepository.GetInventory(from, to);

            //if (inventorySummaryDtosList.Count() < 1) inventorySummaryDtosList = await ProcessInventorySummary(from, to, "", categoryId);
            //else inventorySummaryDtosList = inventorySummaryDtosList.Where(item => item.CategoryId == categoryId);

            //var inventorySummaryDtos = new InventorySummaryDtos();

            //var detailDtosList = new List<InventorySummaryDetailDtos>();

            //var invDetailDtosList = new List<InventorySummaryDetailDtos>();

            //foreach (var item in inventorySummaryDtosList)
            //    detailDtosList.AddRange(item.InventorySummaryDetailDtosList);

            //foreach (var details in detailDtosList.GroupBy(item => item.ItemId))
            //{
            //    var detailList = details.OrderBy(detail => detail.Date);

            //    var item = details.FirstOrDefault().ItemDtos;

            //    invDetailDtosList.Add(new InventorySummaryDetailDtos
            //    {
            //        BeginningInv = details.FirstOrDefault().BeginningInv,
            //        EndingInv = details.LastOrDefault().EndingInv,
            //        ItemDtos = details.LastOrDefault().ItemDtos,
            //        PurchasedItems = details.Sum(detail => detail.PurchasedItems),
            //        PurchaseOrderReturnItems = details.Sum(detail => detail.PurchaseOrderReturnItems),
            //        SalesReturnItems = details.Sum(detail => detail.SalesReturnItems),
            //        SoldItems = details.Sum(detail => detail.SoldItems),
            //        QOH = details.FirstOrDefault().QOH,
            //        CategoryName = item.CategoryName,
            //        PartNo = item.PartNo,
            //        BrandName = item.BrandName,
            //        Made = item.Made,
            //        Make = item.Make,
            //        Model = item.Model,
            //        Size = item.Size
            //    });
            //}

            //if (invDetailDtosList.Count < 1) return null;

            //return new InventorySummaryDtos
            //{
            //    Start = from,
            //    End = to,
            //    CategoryId = categoryId,
            //    CategoryName = invDetailDtosList.FirstOrDefault().CategoryName,
            //    TotalPurchase = invDetailDtosList.Sum(item => item.PurchasedItems),
            //    TotalPurchaseReturn = invDetailDtosList.Sum(item => item.PurchaseOrderReturnItems),
            //    TotalSalesReturn = invDetailDtosList.Sum(item => item.SalesReturnItems),
            //    TotalSold = invDetailDtosList.Sum(item => item.SoldItems),
            //    InventorySummaryDetailDtosList = invDetailDtosList
            //};

            return await ProcessInventorySummary(from, to, "", categoryId);
        }

        #region Helper Method
        private async Task<InventorySummaryDtos> ProcessDetails(DateTime from, DateTime to, string key = "", int categoryId = 0)
        {
            if (this.itemDtosList == null) this.itemDtosList = await itemRepository.GetAll(categoryId);

            IEnumerable<ItemDtos> items = null;

            if (categoryId > 0) items = itemDtosList.Where(item => item.CategoryId == categoryId);
            else items = itemDtosList;

            // Sales Invoice
            var salesInvoiceDtosList = await salesInvoiceController.GetAll(null, from, to);

            if (categoryId > 0) salesInvoiceDtosList = salesInvoiceDtosList.Where(item => item.SalesInvoiceDetailDtosList.Any(detail => detail.ItemDtos.CategoryId == categoryId));

            // Purchase Order
            var purchaseOrderDtosList = await purchaseOrderController.GetAll(null, from, to);

            if (categoryId > 0) purchaseOrderDtosList = purchaseOrderDtosList.Where(item => item.PurchaseOrderDetailDtosList.Any(detail => detail.ItemDtos.CategoryId == categoryId));

            // Sales Return
            var salesReturnDtosList = await salesReturnController.GetAll(null, from, to);

            if (categoryId > 0) salesReturnDtosList = salesReturnDtosList.Where(item => item.SalesReturnDetailDtosList.Any(detail => detail.ItemDtos.CategoryId == categoryId));

            // Purchase Order
            var purchaseOrderReturnDtosList = await purchaseOrderReturnController.GetAll(null, from, to);

            if (categoryId > 0) purchaseOrderReturnDtosList = purchaseOrderReturnDtosList.Where(item => item.PurchaseOrderReturnDetailDtosList.Any(detail => detail.ItemDtos.CategoryId == categoryId));

            var invDtosList = new List<InventorySummaryDetailDtos>();

            if (salesInvoiceDtosList.Count() < 0 && purchaseOrderDtosList.Count() < 0 &&
                salesReturnDtosList.Count() < 0 && purchaseOrderReturnDtosList.Count() < 0) return null;

            //var itemTask = Task.Factory.StartNew(() =>
            //{
            //    Parallel.ForEach(items )
            //});

            foreach (var item in items)//.Where(t => t.ItemId == 20727))
            {
                var invSummary = new InventorySummaryDetailDtos
                {
                    CategoryName = item.CategoryName,
                    ItemDtos = item,
                    QOH = item.QuantityOnHand,
                    ItemId = item.ItemId
                };

                foreach (var sales in salesInvoiceDtosList)
                    invSummary.SoldItems += sales.SalesInvoiceDetailDtosList
                        .Where(detail => detail.ItemDtos.ItemId == item.ItemId).Sum(detail => detail.Quantity);

                foreach (var sales in salesReturnDtosList)
                    invSummary.SalesReturnItems += sales.SalesReturnDetailDtosList
                        .Where(detail => detail.ItemDtos.ItemId == item.ItemId).Sum(detail => detail.Quantity);

                foreach (var puchase in purchaseOrderDtosList)
                    invSummary.PurchasedItems += puchase.PurchaseOrderDetailDtosList
                        .Where(detail => detail.ItemDtos.ItemId == item.ItemId).Sum(detail => detail.Quantity);

                foreach (var puchase in purchaseOrderReturnDtosList)
                    invSummary.PurchaseOrderReturnItems += puchase.PurchaseOrderReturnDetailDtosList
                        .Where(detail => detail.ItemDtos.ItemId == item.ItemId).Sum(detail => detail.Quantity);

                //var inStock = invSummary.PurchasedItems + invSummary.SalesReturnItems;

                //var outStock = invSummary.SoldItems + invSummary.PurchaseOrderReturnItems;

                //invSummary.EndingInv = outStock < inStock ?
                //    inStock - outStock : outStock - inStock;

                //invSummary.EndingInv = item.QuantityOnHand;

                invDtosList.Add(invSummary);
            }

            //Trace.WriteLine(invDtosList.Where(item => item.EndingInv != 0).Count());

            return new InventorySummaryDtos
            {
                Start = from,
                End = to,
                CategoryId = categoryId,
                InventorySummaryDetailDtosList = invDtosList
            };
        }

        private async Task<IEnumerable<InventorySummaryDtos>> ProcessInventorySummary(DateTime from, DateTime to, string key = "", int categoryId = 0)
        {
#region Wrong Code
            /* Wrong
            var lastDate = await invRepository.GetLastInventoryDate();

            this.from = lastDate;

            this.to = to;

            var inventoryDtosList = new List<InventorySummaryDtos>();

            //var first = true;

            var prevDayYesterday = to.AddDays(-1);

            while (lastDate <= to)
            {
                InventorySummaryDtos previousInventory = null;

                var inventorySummaryDtosList = await invRepository.GetInventory(lastDate.AddDays(-1).Date, lastDate.AddDays(-1).Date);

                if (inventorySummaryDtosList.Count() > 0)
                {
                    previousInventory = inventorySummaryDtosList.OrderByDescending(item => item.Start)
                        .FirstOrDefault(inv => inv.Start.Date == lastDate.AddDays(-1).Date && inv.End.Date == lastDate.AddDays(-1).Date);
                }
                else previousInventory = inventoryDtosList.FirstOrDefault(inv => inv.Start.Date == lastDate.AddDays(-1).Date && inv.End.Date == lastDate.AddDays(-1).Date);

                var currentInventory = await ProcessDetails(lastDate, lastDate, "", categoryId);

                if (currentInventory != null)
                {
                    //var invTask = Task.Factory.StartNew(() =>
                    //{
                    //    Parallel.ForEach(currentInventory.InventorySummaryDetailDtosList, mainInv =>
                    //    {
                    //        InventorySummaryDetailDtos begInv = null;

                    //        if (previousInventory != null)
                    //            begInv = previousInventory.InventorySummaryDetailDtosList.FirstOrDefault(inv => inv.ItemDtos.ItemId == mainInv.ItemDtos.ItemId);

                    //        if (begInv != null) mainInv.BeginningInv = begInv.EndingInv;

                    //        //mainInv.BeginningInv = mainInv.QOH - (mainInv.SoldItems + mainInv.PurchasedItems) +
                    //        //    (mainInv.SalesReturnItems + mainInv.PurchasedItems);

                    //        // madama nga stock 
                    //        var inStock = mainInv.PurchasedItems + mainInv.SalesReturnItems;
                    //        //var inStock = mainInv.PurchasedItems + mainInv.BeginningInv;

                    //        var outStock = mainInv.SoldItems + mainInv.PurchaseOrderReturnItems;
                    //        //var outStock = mainInv.SoldItems;

                    //        mainInv.EndingInv = mainInv.BeginningInv - outStock + inStock;
                    //    });
                    //});

                    //invTask.Wait();

                    inventoryDtosList.Add(currentInventory);

                    lastDate = lastDate.AddDays(1);

                    await invRepository.Save(currentInventory);
                }
            }

            foreach (var inventoryDtos in inventoryDtosList.OrderBy(item => item.Start))
            {
                //var currentInventory = await ProcessDetails(inventoryDtos.Start, inventoryDtos.End);

                foreach (var mainInv in inventoryDtos.InventorySummaryDetailDtosList)
                {
                    InventorySummaryDetailDtos begInv = null;

                    //if (previousInventory != null)
                    begInv = inventoryDtos.InventorySummaryDetailDtosList.FirstOrDefault(inv => inv.ItemDtos.ItemId == mainInv.ItemDtos.ItemId);

                    if (begInv != null) mainInv.BeginningInv = begInv.EndingInv;

                    var inStock = mainInv.PurchasedItems + mainInv.SalesReturnItems + mainInv.BeginningInv;

                    var outStock = mainInv.SoldItems + mainInv.PurchaseOrderReturnItems;

                    mainInv.EndingInv = outStock < inStock ?
                        inStock - outStock : outStock - inStock;
                }
            }
             


            return inventoryDtosList.Where(item => item.Start.Date >= from.Date &&
                item.End.Date <= to.Date);
           */
#endregion
            // Sales 
            var sales = await salesInvoiceController.GetAll(null, from: from, to: to);
            if (categoryId > 0) sales = sales.Where(detail => 
                detail.SalesInvoiceDetailDtosList.Any(item => item.ItemDtos.CategoryId == categoryId));

            // Sales returned
            var salesReturned = await salesReturnController.GetAll(null, from: from, to: to);
            if (categoryId > 0) salesReturned = salesReturned.Where(detail => 
                detail.SalesReturnDetailDtosList.Any(item => item.ItemDtos.CategoryId == categoryId));

            // purchased
            var purchased = await purchaseOrderController.GetAll(null, from: from, to: to);
            if (categoryId > 0) purchased = purchased.Where(detail => 
                detail.PurchaseOrderDetailDtosList.Any(item => item.ItemDtos.CategoryId == categoryId));

            // Purchased returned
            var purchasedReturned = await purchaseOrderReturnController.GetAll(null, from: from, to: to);
            if (categoryId > 0) purchasedReturned = purchasedReturned.Where(detail => 
                detail.PurchaseOrderReturnDetailDtosList.Any(item => item.ItemDtos.CategoryId == categoryId));

            var start = from;

            var inventoryList = new List<InventorySummaryDtos>();
            while (start <= to)
            {
                var tmp_purchased = purchased.Where(detail => detail.Date.Date == start.Date);
                var tmp_purchasedReturned = purchasedReturned.Where(detail => detail.Date.Date == start.Date);
                var tmp_salesReturned = salesReturned.Where(detail => detail.Date.Date == start.Date);
                var tmp_sales = sales.Where(detail => detail.Date.Date == start.Date);

                var inventorySummary = new InventorySummaryDtos
                {
                    Start = start,
                    TotalPurchase = tmp_purchased.Sum(detail => detail.TotalQuantity),
                    TotalPurchaseReturn = tmp_purchasedReturned.Sum(detail => detail.TotalQuantity),
                    TotalSalesReturn = tmp_salesReturned.Sum(detail => detail.TotalQuantity),
                    TotalSold = tmp_sales.Sum(detail => detail.TotalQuantity)
                };

                var inventoryDetails = new List<InventorySummaryDetailDtos>();
                var purchasedDetail = tmp_purchased.Select(item => item.PurchaseOrderDetailDtosList).ToList();
                foreach (var item in purchasedDetail) {
                    var detail = inventoryDetails.FirstOrDefault(d => d.ItemId == item.FirstOrDefault().ItemDtos.ItemId);

                    if (detail != null) {
                        detail.PurchasedItems += item.FirstOrDefault().Quantity;
                    }
                    else {
                        addItem(item.FirstOrDefault().ItemDtos, start, ref inventoryDetails, purchased: item.FirstOrDefault().Quantity);
                    }
                }

                var purchaseReturnedDetail = tmp_purchasedReturned.Select(item => item.PurchaseOrderReturnDetailDtosList).ToList();
                foreach (var item in purchaseReturnedDetail)
                {
                    var detail = inventoryDetails.FirstOrDefault(d => d.ItemId == item.FirstOrDefault().ItemDtos.ItemId);

                    if (detail != null)
                    {
                        detail.PurchaseOrderReturnItems += item.FirstOrDefault().Quantity;
                    }
                    else
                    {
                        addItem(item.FirstOrDefault().ItemDtos, start, ref inventoryDetails, purchasedReturn: item.FirstOrDefault().Quantity);
                    }
                }

                var salesReturnedDetail = tmp_salesReturned.Select(item => item.SalesReturnDetailDtosList).ToList();
                foreach (var item in salesReturnedDetail)
                {
                    var detail = inventoryDetails.FirstOrDefault(d => d.ItemId == item.FirstOrDefault().ItemDtos.ItemId);

                    if (detail != null)
                    {
                        detail.SalesReturnItems += item.FirstOrDefault().Quantity;
                    }
                    else
                    {
                        addItem(item.FirstOrDefault().ItemDtos, start, ref inventoryDetails, salesreturned: item.FirstOrDefault().Quantity);
                    }
                }

                var salesDetail = tmp_sales.Select(item => item.SalesInvoiceDetailDtosList).ToList();
                foreach (var item in salesDetail)
                {
                    var detail = inventoryDetails.FirstOrDefault(d => d.ItemId == item.FirstOrDefault().ItemDtos.ItemId);

                    if (detail != null)
                    {
                        detail.SoldItems += item.FirstOrDefault().Quantity;
                    }
                    else
                    {
                        addItem(item.FirstOrDefault().ItemDtos, start, ref inventoryDetails, sold: item.FirstOrDefault().Quantity);
                    }
                }
                start = start.AddDays(1);

                inventorySummary.InventorySummaryDetailDtosList = inventoryDetails;
                inventoryList.Add(inventorySummary);
            }

            return inventoryList;
        }

        private void addItem(ItemDtos item, DateTime date, ref List<InventorySummaryDetailDtos> details, decimal sold = 0, decimal purchased = 0, decimal salesreturned = 0, decimal purchasedReturn = 0)
        {
            details.Add(new InventorySummaryDetailDtos
            {
                BrandName = item.BrandName,
                CategoryName = item.CategoryName,
                PartNo = item.PartNo,
                Made = item.Made,
                Make = item.Make,
                Model = item.Model,
                Date = date,
                SoldItems = sold,
                PurchasedItems = purchased,
                PurchaseOrderReturnItems = purchasedReturn,
                SalesReturnItems = salesreturned,
                ItemId = item.ItemId
            });
        }
        #endregion
    }
}
