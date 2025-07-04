using InventoryServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using InventoryServices.ExtensionMethods;
using CommonLibrary.Dtos;
using InventoryServices.Interfaces;
using System.Runtime.Remoting.Contexts;

namespace InventoryServices.Repositories
{
    public class SalesInvoiceRepository : ISalesInvoiceRepository
    {
        public async Task<bool> Save(SalesInvoiceDtos salesInvoiceDtos, List<int> deletedIdList)
        {
            var dbContext = new InventoryDbContext();

            // Delete purchase order details
            if (deletedIdList != null) await Delete(deletedIdList, dbContext);

            // Save purchase order
            var salesInvoice = salesInvoiceDtos.AsSalesInvoice();

            salesInvoice.User = await FindUser(salesInvoiceDtos.UserId, dbContext);

            if (salesInvoice.CustomerId.HasValue)
            {
                if (salesInvoice.CustomerId.Value > 0)
                    salesInvoice.Customer = await FindCustomer(salesInvoice.CustomerId.Value, dbContext);
                else salesInvoice.CustomerId = null;
            }

            foreach (var detailDtos in salesInvoiceDtos.SalesInvoiceDetailDtosList)
            {
                var detail = detailDtos.AsSalesInvoiceDetail();

                await UpdateQuantityOnHand(detailDtos.ItemDtos.ItemId, 0, detail.Quantity, dbContext);

                var item = await FindItem(detailDtos.ItemDtos.ItemId, dbContext);

                if (item != null)
                {
                    detail.Price2 = item.Price2;

                    detail.CurrentCost = item.CurrentCost;

                    detail.Item = item;

                    detail.CurrentStock = item.QuantityOnHand;
                }

                detail.SalesInvoice = salesInvoice;

                dbContext.SalesInvoiceDetails.Add(detail);
            }

            dbContext.SalesInvoices.Add(salesInvoice);

            return (await dbContext.SaveChangesAsync()) > 0;
        }

        public async Task<SalesInvoiceDtos> FindSalesInvoiceDtos(int id)
        {
            var dbContext = new InventoryDbContext();

            var query = await FindSalesInvoice(id, dbContext);

            if (query == null) return null;

            return query.AsSalesInvoiceDtos();
        }

        public async Task<SalesInvoiceDtos> FindSalesInvoiceDtos(string orNumber)
        {
            var dbContext = new InventoryDbContext();

            var query = await FindSalesInvoice(orNumber, dbContext);

            if (query == null) return null;

            return query.AsSalesInvoiceDtos();
        }

        public async Task<SalesInvoiceDetailDtos> FindSalesInvoiceDetailDtos(int id)
        {
            var dbContext = new InventoryDbContext();

            var query = await FindSalesInvoiceDetail(id, dbContext);

            if (query == null) return null;

            return query.AsSalesInvoiceDetailDtos();
        }

        public async Task<IEnumerable<SalesInvoiceDtos>> GetAll(string key = "")
        {
            using (InventoryDbContext dbContext = new InventoryDbContext())
            {
                int currentYear = DateTime.Now.Year;

                DateTime dateFrom = DateTime.Now.AddMonths(-1);
                DateTime dateTo = DateTime.Now.Date;

                var query = await dbContext.SalesInvoices
                    .Include(order => order.User)
                    .Include(order => order.SalesInvoiceDetailList)
                    .Include(order => order.Customer)
                    .Include(order => order.SalesInvoiceDetailList.Select(detail => detail.Item))
                    .Include(order => order.SalesInvoiceDetailList.Select(detail => detail.Item.Supplier))
                    .Include(order => order.SalesInvoiceDetailList.Select(detail => detail.Item.Category))
                    .Where(order => order.Date >= dateFrom && order.Date <= dateTo && (order.ORNumber.Contains(key) ||
                        order.Customer.CustomerName.Contains(key)) && !order.Returned)
                    .OrderByDescending(order => order.Date)
                    .ToListAsync();

                return query.Select(order => order.AsSalesInvoiceDtos());
            }
        
        }

        public async Task<IEnumerable<SalesInvoiceDtos>> GetByDateRange(DateTime from, DateTime to, string key = "")
        {
            using (InventoryDbContext dbContext = new InventoryDbContext())
            {

                var query = await dbContext.SalesInvoices
                    .Include(item => item.User)
                    .Include(order => order.SalesInvoiceDetailList)
                    .Include(order => order.Customer)
                    .Include(order => order.SalesInvoiceDetailList.Select(detail => detail.Item))
                    .Include(order => order.SalesInvoiceDetailList.Select(detail => detail.Item.Supplier))
                    .Include(order => order.SalesInvoiceDetailList.Select(detail => detail.Item.Category))
                    .Where(order => (order.Date >= from && order.Date <= to) && (order.ORNumber.Contains(key) ||
                        order.Customer.CustomerName.Contains(key)) && !order.Returned)
                    .OrderByDescending(order => order.Date)
                    .ToListAsync();

                return query.Select(order => order.AsSalesInvoiceDtos());
            }

        }

        public async Task<bool> Delete(int id)
        {
            var dbContext = new InventoryDbContext();

            var query = await FindSalesInvoice(id, dbContext);

            dbContext.SalesInvoices.Remove(query);

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
                var detail = await FindSalesInvoiceDetail(id, dbContext);

                if (detail != null)
                {
                    await UpdateQuantityOnHand(detail.Item.Id, detail.Quantity, 0, dbContext);

                    dbContext.SalesInvoiceDetails.Remove(detail);
                }
            }
        }

        private async Task<SalesInvoice> FindSalesInvoice(int id, InventoryDbContext dbContext)
        {
            return await dbContext.SalesInvoices
                .Include(item => item.User)
                .Include(order => order.SalesInvoiceDetailList)
                .Include(order => order.SalesInvoiceDetailList.Select(detail => detail.Item))
                .Include(order => order.SalesInvoiceDetailList.Select(detail => detail.Item.Supplier))
                .Include(order => order.SalesInvoiceDetailList.Select(detail => detail.Item.Category))
                .FirstOrDefaultAsync(order => order.Id == id);
        }

        private async Task<SalesInvoice> FindSalesInvoice(string orNumber, InventoryDbContext dbContext)
        {
            return await dbContext.SalesInvoices
                .Include(item => item.User)
                .Include(order => order.SalesInvoiceDetailList)
                .Include(order => order.SalesInvoiceDetailList.Select(detail => detail.Item))
                .Include(order => order.SalesInvoiceDetailList.Select(detail => detail.Item.Supplier))
                .Include(order => order.SalesInvoiceDetailList.Select(detail => detail.Item.Category))
                .FirstOrDefaultAsync(order => order.ORNumber == orNumber);
        }

        private async Task<Customer> FindCustomer(int id, InventoryDbContext dbContext)
        {
            return await dbContext.Customers
                .FirstOrDefaultAsync(cust => cust.Id == id);
        }

        private async Task<SalesInvoiceDetail> FindSalesInvoiceDetail(int id, InventoryDbContext dbContext)
        {
            return await dbContext.SalesInvoiceDetails
                .Include(order => order.Item)
                .Include(order => order.Item.Supplier)
                .Include(order => order.Item.Category)
                .Include(order => order.SalesInvoice)
                .FirstOrDefaultAsync(order => order.Id == id);
        }

        private async Task<Item> FindItem(int id, InventoryDbContext dbContext)
        {
            return await dbContext.Items
                .Include(item => item.Category)
                .FirstOrDefaultAsync(item => item.Id == id);
        }

        private async Task UpdateQuantityOnHand(int itemId, decimal oldQty, decimal newQty, InventoryDbContext dbContext)
        {
            var queryItem = await FindItem(itemId, dbContext);

            if (queryItem != null)
            {
                queryItem.QuantityOnHand += oldQty;

                queryItem.QuantityOnHand -= newQty;

                dbContext.Entry(queryItem).State = EntityState.Modified;
            }
        }

 
        #endregion
    }
}
