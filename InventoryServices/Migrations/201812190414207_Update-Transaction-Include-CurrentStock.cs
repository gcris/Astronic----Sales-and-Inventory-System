namespace InventoryServices.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTransactionIncludeCurrentStock : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PurchaseOrderDetails", "CurrentStock", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.PurchaseOrderReturnDetails", "CurrentStock", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.SalesInvoiceDetails", "CurrentStock", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.SalesReturnDetails", "CurrentStock", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SalesReturnDetails", "CurrentStock");
            DropColumn("dbo.SalesInvoiceDetails", "CurrentStock");
            DropColumn("dbo.PurchaseOrderReturnDetails", "CurrentStock");
            DropColumn("dbo.PurchaseOrderDetails", "CurrentStock");
        }
    }
}
