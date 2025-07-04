namespace InventoryServices.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSalesInvoiceAllowcustomertobenull : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SalesInvoices", "CustomerId", "dbo.Customers");
            DropIndex("dbo.SalesInvoices", new[] { "CustomerId" });
            AlterColumn("dbo.SalesInvoices", "CustomerId", c => c.Int());
            CreateIndex("dbo.SalesInvoices", "CustomerId");
            AddForeignKey("dbo.SalesInvoices", "CustomerId", "dbo.Customers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SalesInvoices", "CustomerId", "dbo.Customers");
            DropIndex("dbo.SalesInvoices", new[] { "CustomerId" });
            AlterColumn("dbo.SalesInvoices", "CustomerId", c => c.Int(nullable: false));
            CreateIndex("dbo.SalesInvoices", "CustomerId");
            AddForeignKey("dbo.SalesInvoices", "CustomerId", "dbo.Customers", "Id", cascadeDelete: true);
        }
    }
}
