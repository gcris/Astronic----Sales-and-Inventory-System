namespace InventoryServices.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateItemsincludePrices : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Items", "Price1", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Items", "Price2", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Items", "CurrentCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Items", "CurrentCost");
            DropColumn("dbo.Items", "Price2");
            DropColumn("dbo.Items", "Price1");
        }
    }
}
