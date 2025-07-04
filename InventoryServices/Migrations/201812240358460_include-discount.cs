namespace InventoryServices.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class includediscount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PurchaseOrderDetails", "Discount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.PurchaseOrders", "TotalDiscount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.SalesInvoiceDetails", "Discount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.SalesInvoices", "TotalDiscount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SalesInvoices", "TotalDiscount");
            DropColumn("dbo.SalesInvoiceDetails", "Discount");
            DropColumn("dbo.PurchaseOrders", "TotalDiscount");
            DropColumn("dbo.PurchaseOrderDetails", "Discount");
        }
    }
}
