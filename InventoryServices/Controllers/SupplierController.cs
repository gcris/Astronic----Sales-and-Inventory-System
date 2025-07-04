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
    public class SupplierController
    {
        private ISupplierRepository repository = new SupplierRepository();

        public async Task<int> Save(SupplierDtos supplierDtos)
        {
            return await repository.Save(supplierDtos);
        }

        public async Task<SupplierDtos> Find(int id)
        {
            return await repository.FindSupplierDtos(id);
        }

        public async Task<IEnumerable<SupplierDtos>> GetAll(string key = "")
        {
            return await repository.GetAll(key);
        }

        public async Task<bool> Delete(int id)
        {
            return await repository.Delete(id);
        }

        public async Task<bool> IsValidName(string oldName, string newName)
        {
            var queryDtosList = await repository.GetAll();

            var valid = true;

            if (queryDtosList.Count() > 0)
            {
                if (!string.IsNullOrWhiteSpace(oldName))
                    queryDtosList = queryDtosList.Where(cat => cat.Company != oldName);

                valid = !queryDtosList.Any(cat => cat.Company == newName);
            }

            return valid;
        }
    }
}
