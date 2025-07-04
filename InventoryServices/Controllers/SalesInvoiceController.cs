using CommonLibrary.Dtos;
using InventoryServices.Interfaces;
using InventoryServices.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryServices.ExtensionMethods;

namespace InventoryServices.Controllers
{
    public class SalesInvoiceController
    {
        private ISalesInvoiceRepository salesRepository = new SalesInvoiceRepository();
        private IItemRepository itemRepository = new ItemRepository();
        private readonly PurchaseOrderController purchaseOrderController = new PurchaseOrderController();
        private readonly PurchaseOrderReturnController purchaseOrderReturnController = new PurchaseOrderReturnController();
        private readonly SalesReturnController salesReturnController = new SalesReturnController();

        public async Task<IEnumerable<TransactionFrequencyDtos>> GetTransactionFrequency(bool isQuantity = true)
        {
            var thisWeek = DateTime.Now.ThisMonth();

            var list = new List<TransactionFrequencyDtos>();

            var start = thisWeek.Start;

            var salesInvoiceDtosList = await GetAll(null, start.Date, thisWeek.End.Date);

            while (start <= thisWeek.End)
            {
                salesInvoiceDtosList = salesInvoiceDtosList.Where(sales => sales.Date.Date == start.Date);

                list.Add(new TransactionFrequencyDtos
                {
                    Amount = isQuantity ? salesInvoiceDtosList.Sum(item => item.TotalQuantity) :
                        salesInvoiceDtosList.Sum(item => item.TotalAmount),
                    Date = start.Date,
                    Title = isQuantity ? "Sold Item Frequency" : "Sales Item Frequency"
                });

                start = start.AddDays(1);
            }

            return list;
        }

        public async Task<bool> Save(SalesInvoiceDtos salesInvoiceDtos, List<int> deletedIdList)
        {
            salesInvoiceDtos.TotalAmount = salesInvoiceDtos.SalesInvoiceDetailDtosList.Sum(detail => detail.TotalAmount);

            salesInvoiceDtos.TotalQuantity = salesInvoiceDtos.SalesInvoiceDetailDtosList.Sum(detail => detail.Quantity);

            return await salesRepository.Save(salesInvoiceDtos, deletedIdList);
        }

        public async Task<SalesInvoiceDtos> Find(int id)
        {
            return await salesRepository.FindSalesInvoiceDtos(id);
        }

        public async Task<SalesInvoiceDtos> Find(string orNumber)
        {
            return await salesRepository.FindSalesInvoiceDtos(orNumber);
        }

        public async Task<SalesInvoiceDetailDtos> FindDetail(int id)
        {
            return await salesRepository.FindSalesInvoiceDetailDtos(id);
        }

        public async Task<IEnumerable<SalesInvoiceDtos>> GetAll(IEnumerable<SalesInvoiceDtos> salesInvoiceDtosList, DateTime from, DateTime to, string key = "")
        {
             if (salesInvoiceDtosList != null) salesInvoiceDtosList = salesInvoiceDtosList
                .Where(order => order.ORNumber.Contains(key) ||
                    order.CustomerName.Contains(key));
            else if (from > DateTime.MinValue && to > DateTime.MinValue)
            {
                salesInvoiceDtosList = await salesRepository.GetByDateRange(from, to, key);
                //salesInvoiceDtosList = salesInvoiceDtosList.Where(sales => sales.Date.Date >= from.Date && sales.Date.Date <= to.Date).OrderByDescending(sales => sales.Date);
            }
            else
            {
                salesInvoiceDtosList = await salesRepository.GetAll(key);
            }

          
            return salesInvoiceDtosList;
        }



        public async Task<IEnumerable<SalesInvoiceDtos>> GetAllByCustomer(DateTime from, DateTime to, int customerId = 0)
        {
            var list = await salesRepository.GetAll();

            if (from > DateTime.MinValue && to > DateTime.MinValue) list = list.Where(sales => sales.Date.Date >= from && sales.Date.Date <= to.Date);

            if (customerId > 0) list = list.Where(purchase => purchase.CustomerId == customerId);

            return list;
        }

        public async Task<bool> Delete(int id)
        {
            return await salesRepository.Delete(id);
        }

        public async Task<bool> IsValidOrNumber(string orNumber)
        {
            var query = await salesRepository.FindSalesInvoiceDtos(orNumber);

            return query == null;
        }

        public async Task<SalesSummaryDtos> ProcessItemSalesReport(DateTime from, DateTime to, string key = "", int categoryId = 0)
        {
            var items = await itemRepository.GetAll(categoryId, key);

            if (categoryId > 0) items = items.Where(item => item.CategoryId == categoryId);

            var salesInvoices = await GetAll(null, from, to, key);

            if (categoryId > 0) salesInvoices = salesInvoices.Where(item => item.SalesInvoiceDetailDtosList.Any(detail => detail.ItemDtos.CategoryId == categoryId));

            var purchases = await purchaseOrderController.GetAll(null, from, to, key);

            if (categoryId > 0) purchases = purchases.Where(item => item.PurchaseOrderDetailDtosList.Any(detail => detail.ItemDtos.CategoryId == categoryId));

            var salesReturnDtosList = await salesReturnController.GetAll(null, from, to, key);

            if (categoryId > 0) salesReturnDtosList = salesReturnDtosList.Where(item => item.SalesReturnDetailDtosList.Any(detail => detail.ItemDtos.CategoryId == categoryId));

            var purchaseOrderReturnDtosList = await purchaseOrderReturnController.GetAll(null, from, to, key);

            if (categoryId > 0) purchaseOrderReturnDtosList = purchaseOrderReturnDtosList.Where(item => item.PurchaseOrderReturnDetailDtosList.Any(detail => detail.ItemDtos.CategoryId == categoryId));

            var summaryDtosList = new List<SalesSummaryDetailDtos>();

            foreach (var item in items)
            {
                var salesSummary = new SalesSummaryDetailDtos
                {
                    CategoryName = item.CategoryName,
                    ItemDtos = item,
                    PartNo = item.PartNo,
                    BrandName = item.BrandName,
                    Made = item.Made,
                    Make = item.Make,
                    Model = item.Model,
                    Size = item.Size
                };

                foreach (var purchase in purchases)
                {
                    var exists = purchase.PurchaseOrderDetailDtosList.Any(p => p.ItemDtos.ItemId == item.ItemId);

                    if (exists)
                    {
                        salesSummary.PurchasedAmount += purchase.PurchaseOrderDetailDtosList
                            .Where(p => p.ItemDtos.ItemId == item.ItemId).Sum(p => p.TotalAmount);
                    }
                }

                foreach (var sales in salesInvoices)
                {
                    var exists = sales.SalesInvoiceDetailDtosList.Any(p => p.ItemDtos.ItemId == item.ItemId);

                    if (exists)
                    {
                        salesSummary.SoldAmount += sales.SalesInvoiceDetailDtosList
                            .Where(p => p.ItemDtos.ItemId == item.ItemId).Sum(p => p.TotalAmount);
                    }
                }

                foreach (var sales in salesReturnDtosList)
                {
                    var exists = sales.SalesReturnDetailDtosList.Any(p => p.ItemDtos.ItemId == item.ItemId);

                    if (exists)
                    {
                        salesSummary.SalesReturnAmount += sales.SalesReturnDetailDtosList
                            .Where(p => p.ItemDtos.ItemId == item.ItemId).Sum(p => p.Amount);
                    }
                }

                foreach (var sales in purchaseOrderReturnDtosList)
                {
                    var exists = sales.PurchaseOrderReturnDetailDtosList.Any(p => p.ItemDtos.ItemId == item.ItemId);

                    if (exists)
                    {
                        salesSummary.PurchaseOrderReturnAmount += sales.PurchaseOrderReturnDetailDtosList
                            .Where(p => p.ItemDtos.ItemId == item.ItemId).Sum(p => p.Amount);
                    }
                }

                summaryDtosList.Add(salesSummary);
            }

            return new SalesSummaryDtos
            {
                CategoryId = categoryId,
                End = to, 
                Start = from,
                SalesSummaryDetailDtosList = summaryDtosList
            };
        }

        public async Task<SalesIncomeDtos> ProcessSalesIncomeReport(DateTime from, DateTime to, string key = "", int categoryId = 0)
        {
            var items = await itemRepository.GetAll(categoryId, key);

            if (categoryId > 0) items = items.Where(item => item.CategoryId == categoryId);

            var salesInvoices = await GetAll(null, from, to, key);

            if (categoryId > 0) salesInvoices = salesInvoices.Where(item => item.SalesInvoiceDetailDtosList.Any(detail => detail.ItemDtos.CategoryId == categoryId));

            var salesIncomeDetailDtosList = new List<SalesIncomeDetailDtos>();

            foreach (var item in items)
            {
                var salesIncomeDetailDtos = new SalesIncomeDetailDtos
                {
                    CategoryName = item.CategoryName,
                    PartNo = item.PartNo,
                    BrandName = item.BrandName,
                    Made = item.Made,
                    Make = item.Make,
                    Model = item.Model,
                    Size = item.Size
                };

                foreach (var sales in salesInvoices)
                {
                    var exists = sales.SalesInvoiceDetailDtosList.Any(p => p.ItemDtos.ItemId == item.ItemId);

                    if (exists)
                    {
                        salesIncomeDetailDtos.Price1 += sales.SalesInvoiceDetailDtosList
                            .Where(p => p.ItemDtos.ItemId == item.ItemId).Sum(p => p.UnitPrice);

                        salesIncomeDetailDtos.Price2 += sales.SalesInvoiceDetailDtosList
                            .Where(p => p.ItemDtos.ItemId == item.ItemId).Sum(p => p.Price2);

                        salesIncomeDetailDtos.CurrentCost += sales.SalesInvoiceDetailDtosList
                            .Where(p => p.ItemDtos.ItemId == item.ItemId).Sum(p => p.CurrentCost);
                    }
                }

                salesIncomeDetailDtosList.Add(salesIncomeDetailDtos);
            }

            return new SalesIncomeDtos
            {
                CategoryId = categoryId,
                End = to,
                Start = from,
                SalesIncomeDetailDtosList = salesIncomeDetailDtosList
            };
        }
    }
}
