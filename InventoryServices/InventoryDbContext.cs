using InventoryServices.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace InventoryServices
{
    public class InventoryDbContext : DbContext
    {
        //private static readonly string connectionString = "Data Source=transworld-server;Initial Catalog=AstronicAutoSupplyInventoryDb;Integrated Security=False;User Id=admin;Password=systemdeveloper";
        //private static readonly string connectionString = "Data Source=ASTRONIC-SERVER;Initial Catalog=AstronicAutoSupplyInventoryDb;Integrated Security=False;User Id=admin;Password=admin";
        //private static readonly string connectionString = "Data Source=SERVER-PC;Initial Catalog=AstronicAutoSupplyInventoryDb;Integrated Security=False;User Id=admin;Password=admin";

        private static readonly string connectionString = "Data Source=.;Initial Catalog=AstronicAutoSupplyInventoryDb;Integrated Security=True;";

        public InventoryDbContext() : base(connectionString)
        {
            Database.CommandTimeout = 120;
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ItemPrice> ItemPrices { get; set; }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<SalesInvoice> SalesInvoices { get; set; }
        public DbSet<SalesInvoiceDetail> SalesInvoiceDetails { get; set; }

        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        public DbSet<SalesReturn> SalesReturns { get; set; }
        public DbSet<SalesReturnDetail> SalesReturnDetails { get; set; }

        public DbSet<PurchaseOrderReturn> PurchaseOrderReturns { get; set; }
        public DbSet<PurchaseOrderReturnDetail> PurchaseOrderReturnDetails { get; set; }

        public DbSet<InventorySummary> InventorySummaries { get; set; }
        public DbSet<InventorySummaryDetail> InventorySummaryDetails { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<PasswordHistory> PasswordHistories { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserPrivilege> UserPrivileges { get; set; }

        public DbSet<UserActivity> UserActivities { get; set; }
    }
}
