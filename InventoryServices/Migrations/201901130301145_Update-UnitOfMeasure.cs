namespace InventoryServices.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateUnitOfMeasure : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Items", "UnitOfMeasure", c => c.String());
            AlterColumn("dbo.PurchaseOrderDetails", "UOM", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PurchaseOrderDetails", "UOM", c => c.Int(nullable: false));
            AlterColumn("dbo.Items", "UnitOfMeasure", c => c.Int(nullable: false));
        }
    }
}
