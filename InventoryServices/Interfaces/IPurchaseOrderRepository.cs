using CommonLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServices.Interfaces
{
    public interface IPurchaseOrderRepository
    {
        Task<bool> Save(PurchaseOrderDtos purchaseOrderDtos, List<int> deletedIdList);
        Task<PurchaseOrderDtos> FindPurchaseOrderDtos(int id);
        Task<PurchaseOrderDtos> FindPurchaseOrderDtos(string poNumber);
        Task<PurchaseOrderDetailDtos> FindPurchaseOrderDetailDtos(int id);
        Task<IEnumerable<PurchaseOrderDtos>> GetAll(string key = "");
        Task<IEnumerable<PurchaseOrderDtos>> GetByDateRange(DateTime from, DateTime to, string key = "");
        Task<bool> Delete(int id);
    }
}
