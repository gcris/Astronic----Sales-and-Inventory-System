namespace InventoryServices.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSalesInvoiceandSalesReturn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SalesInvoices", "Remarks", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SalesInvoices", "Remarks");
        }
    }
}
