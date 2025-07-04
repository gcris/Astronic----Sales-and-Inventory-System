using CommonLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServices.Interfaces
{
    public interface ISalesReturnRepository
    {
        Task<bool> Save(SalesReturnDtos salesReturnDtos);
        Task<bool> DeleteDetail(int id);
        Task<bool> Delete(int id);
        Task<IEnumerable<SalesReturnDtos>> GetAll(string key = "");
        Task<IEnumerable<SalesReturnDtos>> GetByDateRange(DateTime to, DateTime from, string key = "");
        Task<SalesReturnDtos> FindSalesReturnDtos(int id);
        Task<SalesReturnDtos> FindSalesReturnDtos(string referenceNumber);
    }
}
