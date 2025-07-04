using CommonLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServices.Interfaces
{
    public interface ICustomerRepository
    {
        Task<int> Save(CustomerDtos customerDtos);
        Task<CustomerDtos> FindCustomerDtos(int id);
        Task<IEnumerable<CustomerDtos>> GetAll(string key = "");
        Task<bool> Delete(int id);
    }
}
