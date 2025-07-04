using CommonLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryServices.ExtensionMethods;
using InventoryServices.Models;
using System.Data.Entity;
using InventoryServices.Interfaces;

namespace InventoryServices.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        public async Task<int> Save(CustomerDtos customerDtos)
        {
            var dbContext = new InventoryDbContext();

            var customer = customerDtos.AsCustomer();

            var queryCustomer = await FindCustomer(customer.Id, dbContext);

            if (queryCustomer == null) dbContext.Customers.Add(customer);
            else
            {
                queryCustomer.Address = customer.Address;

                queryCustomer.CustomerName = customer.CustomerName;

                queryCustomer.ContactNo = customer.ContactNo;

                dbContext.Entry(queryCustomer).State = EntityState.Modified;
            }

            await dbContext.SaveChangesAsync();

            return customer.Id;
        }

        public async Task<CustomerDtos> FindCustomerDtos(int id)
        {
            var dbContext = new InventoryDbContext();

            var queryItem = await FindCustomer(id, dbContext);

            if (queryItem == null) return null;

            return queryItem.AsCustomerDtos();
        }

        public async Task<IEnumerable<CustomerDtos>> GetAll(string key = "")
        {
            var dbContext = new InventoryDbContext();

            var queryItemList = await dbContext.Customers
                .Where(cust => cust.Address.Contains(key) ||
                    cust.CustomerName.Contains(key) ||
                    cust.ContactNo.Contains(key)) 
                .OrderBy(cust => cust.CustomerName)
                .ToListAsync();

            return queryItemList.Select(cust => cust.AsCustomerDtos());
        }

        public async Task<bool> Delete(int id)
        {
            var dbContext = new InventoryDbContext();

            var query = await FindCustomer(id, dbContext);

            dbContext.Customers.Remove(query);

            return (await dbContext.SaveChangesAsync()) > 0;
        }

        #region Helper Method
        private async Task<Customer> FindCustomer(int id, InventoryDbContext dbContext)
        {
            return await dbContext.Customers
                .FirstOrDefaultAsync(item => item.Id == id);
        }
        #endregion
    }
}
