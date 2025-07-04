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
    public class PurchaseOrderReturnController
    {
        private IPurchaseOrderReturnRepository repository = new PurchaseOrderReturnRepository();
        private IPurchaseOrderRepository poRepository = new PurchaseOrderRepository();

        public async Task<IEnumerable<TransactionFrequencyDtos>> GetTransactionFrequency(bool isQuantity = true)
        {
            var thisWeek = DateTime.Now.ThisMonth();

            var list = new List<TransactionFrequencyDtos>();

            var start = thisWeek.Start;

            var poReturnDtosList = await GetAll(null, start.Date, thisWeek.End.Date);

            while (start <= thisWeek.End)
            {
                poReturnDtosList = poReturnDtosList.Where(sales => sales.Date.Date == start.Date);

                list.Add(new TransactionFrequencyDtos
                {
                    Amount = isQuantity ? poReturnDtosList.Sum(item => item.TotalQuantity) :
                        poReturnDtosList.Sum(item => item.TotalAmount),
                    Date = start.Date,
                    Title = isQuantity ? "Purchase Return Item Frequency" : "Amount Purchase Return Frequency"
                });

                start = start.AddDays(1);
            }

            return list;
        }

        public async Task<bool> Save(PurchaseOrderReturnDtos purchaseOrderReturnDtos, DateTime date, PurchaseOrderDtos purchaseOrderDtos = null, string remarks = null)
        {
            if (purchaseOrderDtos != null)
            {
                var purchaseOrderReturnDetailDtosList = new List<PurchaseOrderReturnDetailDtos>();

                foreach (var item in purchaseOrderDtos.PurchaseOrderDetailDtosList)
                {
                    var purchaseOrderReturnDetailDtos = new PurchaseOrderReturnDetailDtos
                    {
                        Quantity = item.Quantity,
                        Amount = item.TotalAmount,
                        Remarks = remarks,
                        PurchaseOrderDetailId = item.PurchaseOrderDetailId,
                        ItemDtos = item.ItemDtos
                    };

                    purchaseOrderReturnDetailDtosList.Add(purchaseOrderReturnDetailDtos);
                }

                purchaseOrderReturnDtos = new PurchaseOrderReturnDtos
                {
                    PurchaseOrderReturnDetailDtosList = purchaseOrderReturnDetailDtosList,
                    Date = date,
                    ReferenceNumber = purchaseOrderDtos.PONumber,
                    TotalAmount = purchaseOrderDtos.GrandTotalAmount,
                    TotalQuantity = purchaseOrderDtos.TotalQuantity,
                    UserId = purchaseOrderDtos.UserId
                };
            }
            else
            {
                foreach (var detail in purchaseOrderReturnDtos.PurchaseOrderReturnDetailDtosList)
                {
                    var poDetailDtos = await poRepository.FindPurchaseOrderDetailDtos(detail.PurchaseOrderDetailId);

                    detail.Amount = poDetailDtos.UnitPrice * detail.Quantity;
                }

                purchaseOrderReturnDtos.TotalAmount = purchaseOrderReturnDtos.PurchaseOrderReturnDetailDtosList.Sum(detail => detail.Amount);
            }

            return await repository.Save(purchaseOrderReturnDtos);
        }

        public async Task<bool> Delete(int id, bool byItem = false)
        {
            if (byItem) return await repository.DeleteDetail(id);
            else return await repository.Delete(id);
        }

        public async Task<PurchaseOrderReturnDtos> Find(int id)
        {
            return await repository.FindPurchaseOrderReturn(id);
        }

        public async Task<PurchaseOrderReturnDtos> Find(string referenceNumber)
        {
            return await repository.FindPurchaseOrderReturn(referenceNumber);
        }

        public async Task<IEnumerable<PurchaseOrderReturnDtos>> GetAll(IEnumerable<PurchaseOrderReturnDtos> poDtosList, DateTime from, DateTime to, string key = "")
        {
            if (poDtosList != null) poDtosList = poDtosList
                .Where(order => order.ReferenceNumber.Contains(key));
            else if (from > DateTime.MinValue && to > DateTime.MinValue)
            {
                poDtosList = await repository.GetByDateRange(from, to, key);
                //poDtosList = poDtosList.Where(sales => sales.Date.Date >= from.Date && sales.Date.Date <= to.Date).OrderByDescending(sales => sales.Date);
            }
            else poDtosList = await repository.GetAll(key);
          
            return poDtosList;
        }
    }
}
