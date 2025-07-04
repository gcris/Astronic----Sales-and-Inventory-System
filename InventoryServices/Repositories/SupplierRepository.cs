using CommonLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using InventoryServices.ExtensionMethods;
using InventoryServices.Models;
using InventoryServices.Interfaces;

namespace InventoryServices.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        public async Task<int> Save(SupplierDtos supplierDtos)
        {
            var dbContext = new InventoryDbContext();

            var supplieromer = supplierDtos.AsSupplier();

            var querysupplieromer = await FindSupplier(supplieromer.Id, dbContext);

            if (querysupplieromer == null) dbContext.Suppliers.Add(supplieromer);
            else
            {
                querysupplieromer.Address = supplieromer.Address;

                querysupplieromer.Company = supplieromer.Company;

                querysupplieromer.ContactPerson = supplieromer.ContactPerson;

                querysupplieromer.ContactNo = supplieromer.ContactNo;

                dbContext.Entry(querysupplieromer).State = EntityState.Modified;
            }

            await dbContext.SaveChangesAsync();

            return supplieromer.Id;
        }

        public async Task<SupplierDtos> FindSupplierDtos(int id)
        {
            var dbContext = new InventoryDbContext();

            var queryItem = await FindSupplier(id, dbContext);

            if (queryItem == null) return null;

            return queryItem.AsSupplierDtos();
        }

        public async Task<IEnumerable<SupplierDtos>> GetAll(string key = "")
        {
            var dbContext = new InventoryDbContext();

            var queryItemList = await dbContext.Suppliers
                .Where(supplier => supplier.Address.Contains(key) ||
                    supplier.ContactPerson.Contains(key) ||
                    supplier.ContactNo.Contains(key) ||
                    supplier.Company.Contains(key))
                .OrderBy(supplier => supplier.Company)
                .ToListAsync();

            return queryItemList.Select(supplier => supplier.AsSupplierDtos());
        }

        public async Task<bool> Delete(int id)
        {
            var dbContext = new InventoryDbContext();

            var query = await FindSupplier(id, dbContext);

            dbContext.Suppliers.Remove(query);

            return (await dbContext.SaveChangesAsync()) > 0;
        }

        #region Helper Method
        private async Task<Supplier> FindSupplier(int id, InventoryDbContext dbContext)
        {
            return await dbContext.Suppliers
                .FirstOrDefaultAsync(item => item.Id == id);
        }
        #endregion
    }
}
