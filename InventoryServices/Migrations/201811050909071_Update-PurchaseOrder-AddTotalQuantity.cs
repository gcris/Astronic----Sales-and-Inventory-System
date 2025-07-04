namespace InventoryServices.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePurchaseOrderAddTotalQuantity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PurchaseOrders", "TotalQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PurchaseOrders", "TotalQuantity");
        }
    }
}
