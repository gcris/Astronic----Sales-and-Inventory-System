using CommonLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServices.Interfaces
{
    public interface ISalesInvoiceRepository
    {
        Task<bool> Save(SalesInvoiceDtos salesInvoiceDtos, List<int> deletedIdList);
        Task<SalesInvoiceDtos> FindSalesInvoiceDtos(int id);
        Task<SalesInvoiceDtos> FindSalesInvoiceDtos(string orNumber);
        Task<SalesInvoiceDetailDtos> FindSalesInvoiceDetailDtos(int id);
        Task<IEnumerable<SalesInvoiceDtos>> GetAll(string key = "");
        Task<IEnumerable<SalesInvoiceDtos>> GetByDateRange(DateTime from, DateTime to,string key = "");
        Task<bool> Delete(int id);
    }
}
