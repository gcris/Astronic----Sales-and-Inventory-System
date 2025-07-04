using CommonLibrary.Dtos;
using InventoryServices.Interfaces;
using InventoryServices.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryServices.ExtensionMethods;
using InventoryServices.EventMessenger;

namespace InventoryServices.Controllers
{
    public class PurchaseOrderController
    {
        private IPurchaseOrderRepository orderRepository = new PurchaseOrderRepository();
        private IItemRepository itemRepository = new ItemRepository();

        public async Task<IEnumerable<TransactionFrequencyDtos>> GetTransactionFrequency(bool isQuantity = true)
        {
            var thisWeek = DateTime.Now.ThisMonth();

            var list = new List<TransactionFrequencyDtos>();

            var start = thisWeek.Start;

            var purchaseOrderDtosList = await GetAll(null, start.Date, thisWeek.End.Date);

            while (start <= thisWeek.End)
            {
                purchaseOrderDtosList = purchaseOrderDtosList.Where(sales => sales.Date.Date == start.Date);

                list.Add(new TransactionFrequencyDtos
                {
                    Amount = isQuantity ? purchaseOrderDtosList.Sum(item => item.TotalQuantity) :
                        purchaseOrderDtosList.Sum(item => item.GrandTotalAmount),
                    Date = start.Date,
                    Title = isQuantity ? "Purchase Item Frequency" : "Amount Purchase Frequency"
                });

                start = start.AddDays(1);
            }

            return list;
        }

        public async Task<bool> Save(PurchaseOrderDtos purchaseOrderDtos, List<int> deletedIdList, NotifierEventMessenger notifierEventMessenger)
        {
            purchaseOrderDtos.GrandTotalAmount = purchaseOrderDtos.PurchaseOrderDetailDtosList.Sum(detail => detail.TotalAmount);

            purchaseOrderDtos.TotalQuantity = purchaseOrderDtos.PurchaseOrderDetailDtosList.Sum(detail => detail.Quantity);

            var success = await orderRepository.Save(purchaseOrderDtos, deletedIdList);

            if (success) notifierEventMessenger(success);

            return success;
        }

        public async Task<PurchaseOrderDtos> Find(int id)
        {
            return await orderRepository.FindPurchaseOrderDtos(id);
        }

        public async Task<PurchaseOrderDtos> Find(string poNumber)
        {
            return await orderRepository.FindPurchaseOrderDtos(poNumber);
        }

        public async Task<PurchaseOrderDetailDtos> FindDetail(int id)
        {
            return await orderRepository.FindPurchaseOrderDetailDtos(id);
        }

        public async Task<IEnumerable<PurchaseOrderDtos>> GetAll(IEnumerable<PurchaseOrderDtos> poDtosList, DateTime from, DateTime to, string key = "")
        {
            if (poDtosList != null) poDtosList = poDtosList
                .Where(order => order.PONumber.Contains(key) ||
                    order.SupplierName.Contains(key));
            else if (from > DateTime.MinValue && to > DateTime.MinValue)
            {
                poDtosList = await orderRepository.GetByDateRange(from, to, key);
                //poDtosList = poDtosList.Where(purchase => purchase.Date.Date >= from.Date && purchase.Date.Date <= to.Date);
            }
            else poDtosList = await orderRepository.GetAll(key);

            return poDtosList;
        }

        public async Task<IEnumerable<PurchaseOrderDtos>> GetAllBySupplier(DateTime from, DateTime to, int supplierId = 0)
        {
            var list = await orderRepository.GetAll();

            if (from > DateTime.MinValue && to > DateTime.MinValue) list = list.Where(purchase => purchase.Date.Date >= from && purchase.Date.Date <= to.Date);

            if (supplierId > 0) list = list.Where(purchase => purchase.SupplierId == supplierId);

            return list;
        }

        public async Task<bool> IsValidPoNumber(string poNumber)
        {
            var query = await orderRepository.FindPurchaseOrderDtos(poNumber);

            return query == null;
        }

        public async Task<bool> Delete(int id)
        {
            return await orderRepository.Delete(id);
        }
    }
}
