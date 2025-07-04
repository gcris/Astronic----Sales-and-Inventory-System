using CommonLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServices.Interfaces
{
    public interface ISupplierRepository
    {
        Task<int> Save(SupplierDtos supplierDtos);
        Task<SupplierDtos> FindSupplierDtos(int id);
        Task<IEnumerable<SupplierDtos>> GetAll(string key = "");
        Task<bool> Delete(int id);
    }
}
