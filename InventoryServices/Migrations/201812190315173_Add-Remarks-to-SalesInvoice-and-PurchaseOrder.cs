namespace InventoryServices.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRemarkstoSalesInvoiceandPurchaseOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PurchaseOrders", "Remarks", c => c.String());
            AddColumn("dbo.SalesInvoices", "Remarks", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SalesInvoices", "Remarks");
            DropColumn("dbo.PurchaseOrders", "Remarks");
        }
    }
}
