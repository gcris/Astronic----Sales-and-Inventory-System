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
    public class CustomerController
    {
        private ICustomerRepository repository = new CustomerRepository();

        public async Task<int> Save(CustomerDtos customerDtos)
        {
            return await repository.Save(customerDtos);
        }

        public async Task<CustomerDtos> Find(int id)
        {
            return await repository.FindCustomerDtos(id);
        }

        public async Task<IEnumerable<CustomerDtos>> GetAll(string key = "")
        {
            return await repository.GetAll(key);
        }

        public async Task<bool> Delete(int id)
        {
            return await repository.Delete(id);
        }
    }
}
