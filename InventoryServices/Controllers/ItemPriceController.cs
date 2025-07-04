using CommonLibrary.Dtos;
using InventoryServices.EventMessenger;
using InventoryServices.Interfaces;
using InventoryServices.Models;
using InventoryServices.Repositories;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServices.Controllers
{
    public class ItemPriceController
    {
        private IItemPriceRepository repository = new ItemPriceRepository();

        //private SqlDependencyNotification<ItemPrice> sqlNotification;

        public async Task<bool> Save(IEnumerable<ItemPriceDtos> itemPriceDtosList, NotifierEventMessenger notifierEventMessenger)
        {
            var success = false;

            //sqlNotification = new SqlDependencyNotification<ItemPrice>();

            //sqlNotification.StartSqlDependency();

            //sqlNotification.InitiateNotifier((object senderArgs, SqlNotificationEventArgs eventArg) =>
            //{
            //    try
            //    {
                    
            //    }
            //    catch (Exception ex)
            //    {
            //        // Do something about exception here. 
            //        Trace.WriteLine(ex.Message);
            //    }
            //});

            foreach (var itemPriceDtos in itemPriceDtosList)
            {
                success = await repository.Save(itemPriceDtos);

                if (!success) break;
            }

            //sqlNotification.TerminateSqlDependency();

            notifierEventMessenger(success);

            return success;
        }

        public async Task<IEnumerable<ItemPriceDtos>> GetPriceListByItem(int id)
        {
            return await repository.GetPriceListByItem(id);
        }

        public async Task<IEnumerable<ItemPriceDtos>> GetPriceListByDate(int categoryId, DateTime date)
        {
            return await repository.GetPriceListByDate(categoryId, date);
        }
    }
}
