namespace InventoryServices.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTransactionsaddUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PurchaseOrders", "UserId", c => c.Int(nullable: false, identity: false, defaultValue: 1));
            AddColumn("dbo.SalesInvoices", "UserId", c => c.Int(nullable: false, identity: false, defaultValue: 1));
            AddColumn("dbo.PurchaseOrderReturns", "UserId", c => c.Int(nullable: false, identity: false, defaultValue: 1));
            AddColumn("dbo.SalesReturns", "UserId", c => c.Int(nullable: false, identity: false, defaultValue: 1));
            CreateIndex("dbo.PurchaseOrders", "UserId");
            CreateIndex("dbo.SalesInvoices", "UserId");
            CreateIndex("dbo.PurchaseOrderReturns", "UserId");
            CreateIndex("dbo.SalesReturns", "UserId");
            AddForeignKey("dbo.PurchaseOrders", "UserId", "dbo.Users", "Id");
            AddForeignKey("dbo.SalesInvoices", "UserId", "dbo.Users", "Id");
            AddForeignKey("dbo.PurchaseOrderReturns", "UserId", "dbo.Users", "Id");
            AddForeignKey("dbo.SalesReturns", "UserId", "dbo.Users", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SalesReturns", "UserId", "dbo.Users");
            DropForeignKey("dbo.PurchaseOrderReturns", "UserId", "dbo.Users");
            DropForeignKey("dbo.SalesInvoices", "UserId", "dbo.Users");
            DropForeignKey("dbo.PurchaseOrders", "UserId", "dbo.Users");
            DropIndex("dbo.SalesReturns", new[] { "UserId" });
            DropIndex("dbo.PurchaseOrderReturns", new[] { "UserId" });
            DropIndex("dbo.SalesInvoices", new[] { "UserId" });
            DropIndex("dbo.PurchaseOrders", new[] { "UserId" });
            DropColumn("dbo.SalesReturns", "UserId");
            DropColumn("dbo.PurchaseOrderReturns", "UserId");
            DropColumn("dbo.SalesInvoices", "UserId");
            DropColumn("dbo.PurchaseOrders", "UserId");
        }
    }
}
