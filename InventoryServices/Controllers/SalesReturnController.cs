using CommonLibrary.Dtos;
using InventoryServices.Interfaces;
using InventoryServices.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryServices.ExtensionMethods;

namespace InventoryServices.Controllers
{
    public class SalesReturnController
    {
        private ISalesReturnRepository repository = new SalesReturnRepository();
        private ISalesInvoiceRepository salesInvoiceRepository = new SalesInvoiceRepository();

        public async Task<IEnumerable<TransactionFrequencyDtos>> GetTransactionFrequency(bool isQuantity = true)
        {
            var thisWeek = DateTime.Now.ThisMonth();

            var list = new List<TransactionFrequencyDtos>();

            var start = thisWeek.Start;

            var salesReturnDtosList = await GetAll(null, start.Date, thisWeek.End.Date);

            while (start <= thisWeek.End)
            {
                salesReturnDtosList = salesReturnDtosList.Where(sales => sales.Date.Date == start.Date);

                list.Add(new TransactionFrequencyDtos
                {
                    Amount = isQuantity ? salesReturnDtosList.Sum(item => item.TotalQuantity) :
                        salesReturnDtosList.Sum(item => item.TotalAmount),
                    Date = start.Date,
                    Title = isQuantity ? "Sales Return Item Frequency" : "Amount Sales Return Frequency"
                });

                start = start.AddDays(1);
            }

            return list;
        }

        public async Task<bool> Save(SalesReturnDtos salesReturnDtos, DateTime date, SalesInvoiceDtos salesInvoiceDtos = null, string remarks = null)
        {
            if (salesInvoiceDtos != null)
            {
                var salesReturnDetailList = new List<SalesReturnDetailDtos>();

                foreach (var item in salesInvoiceDtos.SalesInvoiceDetailDtosList)
                {
                    var salesReturnDetail = new SalesReturnDetailDtos
                    {
                        Quantity = item.Quantity,
                        Amount = item.TotalAmount,
                        Remarks = remarks,
                        SalesInvoiceDetailId = item.SalesInvoiceDetailId,
                        ItemDtos = item.ItemDtos
                    };

                    salesReturnDetailList.Add(salesReturnDetail);
                }

                salesReturnDtos = new SalesReturnDtos
                {
                    SalesReturnDetailDtosList = salesReturnDetailList,
                    Date = date,
                    ReferenceNumber = salesInvoiceDtos.ORNumber,
                    TotalAmount = salesInvoiceDtos.TotalAmount,
                    TotalQuantity = salesInvoiceDtos.TotalQuantity,
                    UserId = salesInvoiceDtos.UserId
                };
            }
            else
            {
                foreach (var detail in salesReturnDtos.SalesReturnDetailDtosList)
                {
                    var salesInvoiceDetailDtos = await salesInvoiceRepository.FindSalesInvoiceDetailDtos(detail.SalesInvoiceDetailId);

                    detail.Amount = salesInvoiceDetailDtos.UnitPrice * detail.Quantity;
                }

                salesReturnDtos.TotalAmount = salesReturnDtos.SalesReturnDetailDtosList.Sum(detail => detail.Amount);
            }

            return await repository.Save(salesReturnDtos);
        }

        public async Task<bool> Delete(int id, bool byItem = false)
        {
            if (byItem) return await repository.DeleteDetail(id);
            else return await repository.Delete(id);
        }

        public async Task<SalesReturnDtos> Find(int id)
        {
            return await repository.FindSalesReturnDtos(id);
        }

        public async Task<SalesReturnDtos> Find(string referenceNumber)
        {
            return await repository.FindSalesReturnDtos(referenceNumber);
        }

        public async Task<IEnumerable<SalesReturnDtos>> GetAll(IEnumerable<SalesReturnDtos> salesReturnDtosList, DateTime from, DateTime to, string key = "")
        {
            if (salesReturnDtosList != null)
                salesReturnDtosList = salesReturnDtosList.Where(item => item.ReferenceNumber.Contains(key));
            else if (from > DateTime.MinValue && to > DateTime.MinValue)
            {
                salesReturnDtosList = await repository.GetByDateRange(from, to, key);
                //salesReturnDtosList = salesReturnDtosList.Where(sales => sales.Date.Date >= from.Date && sales.Date.Date <= to.Date).OrderByDescending(sales => sales.Date);
            }
            else salesReturnDtosList = await repository.GetAll(key);

            return salesReturnDtosList;
        }
    }
}
