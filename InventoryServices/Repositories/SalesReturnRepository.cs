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
    public class SalesReturnRepository : ISalesReturnRepository
    {
        public async Task<bool> Save(SalesReturnDtos salesReturnDtos)
        {
            var dbContext = new InventoryDbContext();

            var salesReturn = salesReturnDtos.AsSalesReturn();

            salesReturn.User = await FindUser(salesReturnDtos.UserId, dbContext);

            foreach (var salesReturnDetailDtos in salesReturnDtos.SalesReturnDetailDtosList)
            {
                var salesReturnDetail = salesReturnDetailDtos.AsSalesReturnDetail();

                await UpdateQuantityOnHand(
                    salesReturnDetailDtos.ItemDtos.ItemId,
                    salesReturnDetailDtos.Quantity,
                    0,
                    salesReturn.Date,
                    dbContext);

                var salesInvoiceDetail = await FindSalesInvoiceDetail(salesReturnDetailDtos.SalesInvoiceDetailId, dbContext);

                if (salesInvoiceDetail != null)
                {
                    salesInvoiceDetail.Quantity -= salesReturnDetail.Quantity;

                    salesInvoiceDetail.TotalAmount = salesInvoiceDetail.Quantity * salesInvoiceDetail.UnitPrice;

                    dbContext.Entry(salesInvoiceDetail).State = EntityState.Modified;

                    if (salesInvoiceDetail.SalesInvoice != null)
                    {
                        var salesInvoice = await FindSalesInvoice(salesInvoiceDetail.SalesInvoiceId, dbContext);

                        salesInvoice.TotalAmount = salesInvoice.SalesInvoiceDetailList.Sum(detail => detail.TotalAmount);

                        salesInvoice.TotalQuantity = salesInvoice.SalesInvoiceDetailList.Sum(detail => detail.Quantity);

                        salesInvoice.Returned = salesInvoice.TotalAmount == 0 && salesInvoice.TotalQuantity == 0;

                        dbContext.Entry(salesInvoice).State = EntityState.Modified;
                    }
                }

                salesReturnDetail.SalesInvoiceDetail = salesInvoiceDetail;

                salesReturnDetail.SalesReturn = salesReturn;

                salesReturnDetail.CurrentStock = salesReturnDetail.SalesInvoiceDetail.Item.QuantityOnHand;

                dbContext.SalesReturnDetails.Add(salesReturnDetail);
            }

            dbContext.SalesReturns.Add(salesReturn);

            return (await dbContext.SaveChangesAsync()) > 0;
        }

        public async Task<IEnumerable<SalesReturnDtos>> GetAll(string key = "")
        {
            using (InventoryDbContext dbContext = new InventoryDbContext())
            {
                int currentYear = DateTime.Now.Year;


                DateTime dateFrom = DateTime.Now.AddMonths(-1);
                DateTime dateTo = DateTime.Now.Date;

                var queryList = await dbContext.SalesReturns
                .Include(item => item.User)
                .Include(item => item.SalesReturnDetailList)
                .Include(item => item.SalesReturnDetailList.Select(detail => detail.SalesInvoiceDetail.Item))
                .Include(item => item.SalesReturnDetailList.Select(detail => detail.SalesInvoiceDetail.Item.Category))
                .Include(item => item.SalesReturnDetailList.Select(detail => detail.SalesInvoiceDetail.SalesInvoice))
                .Where(item => item.Date >= dateFrom && item.Date <= dateTo && item.ReferenceNumber.Contains(key) && item.Date.Year == currentYear)
                .OrderByDescending(item => item.Date)
                .ToListAsync();

                return queryList.Select(item => item.AsSalesReturnDtos());
            }
        }
        public async Task<IEnumerable<SalesReturnDtos>> GetByDateRange(DateTime from, DateTime to, string key = "")
        {
            using (InventoryDbContext dbContext = new InventoryDbContext())
            {

                var queryList = await dbContext.SalesReturns
                .Include(item => item.User)
                .Include(item => item.SalesReturnDetailList)
                .Include(item => item.SalesReturnDetailList.Select(detail => detail.SalesInvoiceDetail.Item))
                .Include(item => item.SalesReturnDetailList.Select(detail => detail.SalesInvoiceDetail.Item.Category))
                .Include(item => item.SalesReturnDetailList.Select(detail => detail.SalesInvoiceDetail.SalesInvoice))
                .Where(item => (item.Date >= from && item.Date <= to) && item.ReferenceNumber.Contains(key))
                //.OrderByDescending(item => item.Date)
                .ToListAsync();

                return queryList.Select(item => item.AsSalesReturnDtos());
            }
        }

        public async Task<SalesReturnDtos> FindSalesReturnDtos(int id)
        {
            var dbContext = new InventoryDbContext();

            var querySalesReturn = await FindSalesReturn(id, dbContext);

            if (querySalesReturn == null) return null;

            return querySalesReturn.AsSalesReturnDtos();
        }

        public async Task<SalesReturnDtos> FindSalesReturnDtos(string referenceNumber)
        {
            var dbContext = new InventoryDbContext();

            var querySalesReturn = await FindSalesReturn(referenceNumber, dbContext);

            if (querySalesReturn == null) return null;

            return querySalesReturn.AsSalesReturnDtos();
        }

        public async Task<bool> DeleteDetail(int id)
        {
            var dbContext = new InventoryDbContext();

            var queryDetail = await FindSalesReturnDetail(id, dbContext);

            if (queryDetail != null)
            {
                var salesReturnId = queryDetail.SalesInvoiceDetailId;

                await UpdateQuantityOnHand(queryDetail.SalesInvoiceDetail.ItemId,
                    0,
                    queryDetail.Quantity,
                    DateTime.Now,
                    dbContext);

                var salesInvoiceDetail = await FindSalesInvoiceDetail(queryDetail.SalesInvoiceDetailId, dbContext);

                if (salesInvoiceDetail != null)
                {
                    salesInvoiceDetail.Quantity += queryDetail.Quantity;

                    salesInvoiceDetail.TotalAmount = salesInvoiceDetail.Quantity * salesInvoiceDetail.UnitPrice;

                    dbContext.Entry(salesInvoiceDetail).State = EntityState.Modified;

                    var salesInvoice = await FindSalesInvoice(salesInvoiceDetail.SalesInvoiceId, dbContext);

                    if (salesInvoice != null)
                    {
                        salesInvoice.TotalQuantity = salesInvoice.SalesInvoiceDetailList.Sum(detail => detail.Quantity);

                        salesInvoice.TotalAmount = salesInvoice.SalesInvoiceDetailList.Sum(detail => detail.TotalAmount);

                        salesInvoice.Returned = salesInvoice.TotalAmount == 0 && salesInvoice.TotalQuantity == 0;

                        dbContext.Entry(salesInvoice).State = EntityState.Modified;
                    }
                }

                dbContext.SalesReturnDetails.Remove(queryDetail);

                var salesReturn = await FindSalesReturn(salesReturnId, dbContext);

                if (salesReturn != null)
                {
                    if (salesReturn.SalesReturnDetailList == null)
                    {
                        dbContext.SalesReturns.Remove(salesReturn);
                    }
                }
            }

            return (await dbContext.SaveChangesAsync()) > 0;
        }

        public async Task<bool> Delete(int id)
        {
            var dbContext = new InventoryDbContext();

            var querySalesReturn = await FindSalesReturn(id, dbContext);

            if (querySalesReturn != null)
            {
                foreach (var queryDetail in querySalesReturn.SalesReturnDetailList)
                {
                    await UpdateQuantityOnHand(queryDetail.SalesInvoiceDetail.ItemId,
                        0,
                        queryDetail.Quantity,
                        DateTime.Now,
                        dbContext);

                    var salesInvoiceDetail = await FindSalesInvoiceDetail(queryDetail.SalesInvoiceDetailId, dbContext);

                    if (salesInvoiceDetail != null)
                    {
                        salesInvoiceDetail.Quantity += queryDetail.Quantity;

                        salesInvoiceDetail.TotalAmount = salesInvoiceDetail.Quantity * salesInvoiceDetail.UnitPrice;

                        dbContext.Entry(salesInvoiceDetail).State = EntityState.Modified;

                        var salesInvoice = await FindSalesInvoice(salesInvoiceDetail.SalesInvoiceId, dbContext);

                        if (salesInvoice != null)
                        {
                            salesInvoice.TotalQuantity = salesInvoice.SalesInvoiceDetailList.Sum(detail => detail.Quantity);

                            salesInvoice.TotalAmount = salesInvoice.SalesInvoiceDetailList.Sum(detail => detail.TotalAmount);

                            salesInvoice.Returned = salesInvoice.TotalAmount == 0 && salesInvoice.TotalQuantity == 0;

                            dbContext.Entry(salesInvoice).State = EntityState.Modified;
                        }
                    }
                }

                dbContext.SalesReturns.Remove(querySalesReturn);
            }

            return (await dbContext.SaveChangesAsync()) > 0;
        }

        private async Task<SalesReturn> FindSalesReturn(int id, InventoryDbContext dbContext)
        {
            return await dbContext.SalesReturns
                .Include(item => item.User)
                .Include(item => item.SalesReturnDetailList)
                .Include(item => item.SalesReturnDetailList.Select(detail => detail.SalesInvoiceDetail))
                .Include(item => item.SalesReturnDetailList.Select(detail => detail.SalesInvoiceDetail.Item))
                .FirstOrDefaultAsync(item => item.Id == id);
        }

        private async Task<SalesReturn> FindSalesReturn(string referenceNumber, InventoryDbContext dbContext)
        {
            return await dbContext.SalesReturns
                .Include(item => item.User)
                .Include(item => item.SalesReturnDetailList)
                .Include(item => item.SalesReturnDetailList.Select(detail => detail.SalesInvoiceDetail))
                .Include(item => item.SalesReturnDetailList.Select(detail => detail.SalesInvoiceDetail.Item))
                .FirstOrDefaultAsync(item => item.ReferenceNumber == referenceNumber);
        }

        #region Helper
        private async Task<User> FindUser(int id, InventoryDbContext dbContext)
        {
            return await dbContext.Users.FirstOrDefaultAsync(user => user.Id == id);
        }

        private async Task<SalesReturnDetail> FindSalesReturnDetail(int id, InventoryDbContext dbContext)
        {
            return await dbContext.SalesReturnDetails
                .Include(item => item.SalesInvoiceDetail)
                .Include(item => item.SalesInvoiceDetail.SalesInvoice)
                .Include(item => item.SalesInvoiceDetail.Item)
                .FirstOrDefaultAsync(item => item.Id == id);
        }

        private async Task<SalesInvoiceDetail> FindSalesInvoiceDetail(int id, InventoryDbContext dbContext)
        {
            return await dbContext.SalesInvoiceDetails.FirstOrDefaultAsync(item => item.Id == id);
        }

        private async Task<SalesInvoice> FindSalesInvoice(int id, InventoryDbContext dbContext)
        {
            return await dbContext.SalesInvoices.FirstOrDefaultAsync(item => item.Id == id);
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
