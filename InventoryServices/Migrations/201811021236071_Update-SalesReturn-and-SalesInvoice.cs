namespace InventoryServices.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSalesReturnandSalesInvoice : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.SalesInvoices", "Remarks");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SalesInvoices", "Remarks", c => c.String());
        }
    }
}
