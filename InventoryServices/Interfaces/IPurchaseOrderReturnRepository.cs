using CommonLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServices.Interfaces
{
    public interface IPurchaseOrderReturnRepository
    {
        Task<bool> Save(PurchaseOrderReturnDtos salesReturnDtos);
        Task<bool> DeleteDetail(int id);
        Task<bool> Delete(int id);
        Task<IEnumerable<PurchaseOrderReturnDtos>> GetAll(string key = "");
        Task<IEnumerable<PurchaseOrderReturnDtos>> GetByDateRange(DateTime from, DateTime to, string key = "");
        Task<PurchaseOrderReturnDtos> FindPurchaseOrderReturn(int id);
        Task<PurchaseOrderReturnDtos> FindPurchaseOrderReturn(string referenceNumber);
    }
}
