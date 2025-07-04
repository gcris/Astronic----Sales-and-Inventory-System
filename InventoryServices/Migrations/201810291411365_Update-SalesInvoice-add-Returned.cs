namespace InventoryServices.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSalesInvoiceaddReturned : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SalesInvoices", "Returned", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SalesInvoices", "Returned");
        }
    }
}
