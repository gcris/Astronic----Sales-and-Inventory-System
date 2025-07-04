namespace InventoryServices.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSalesReturnaddTotalAmount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SalesReturnDetails", "Amount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.SalesReturns", "TotalAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SalesReturns", "TotalAmount");
            DropColumn("dbo.SalesReturnDetails", "Amount");
        }
    }
}
