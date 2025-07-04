namespace InventoryServices.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSalesInvoiceincludeallprices : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SalesInvoiceDetails", "Price2", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.SalesInvoiceDetails", "CurrentCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SalesInvoiceDetails", "CurrentCost");
            DropColumn("dbo.SalesInvoiceDetails", "Price2");
        }
    }
}
