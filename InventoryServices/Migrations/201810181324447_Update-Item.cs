namespace InventoryServices.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Items", "Size", c => c.String());
            AddColumn("dbo.Items", "QuantityOnHand", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Items", "LastUpdate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Items", "LastUpdate");
            DropColumn("dbo.Items", "QuantityOnHand");
            DropColumn("dbo.Items", "Size");
        }
    }
}
