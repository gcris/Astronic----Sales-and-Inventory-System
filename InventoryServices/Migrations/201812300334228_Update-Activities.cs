namespace InventoryServices.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateActivities : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserActivities", "ReferenceNumber", c => c.String());
            AddColumn("dbo.UserActivities", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.UserActivities", "Amount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.UserActivities", "CurrentStock", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.UserActivities", "Remarks", c => c.String());
            AddColumn("dbo.UserActivities", "Transaction", c => c.Boolean(nullable: false));
            AddColumn("dbo.UserActivities", "ItemId", c => c.Int());
            CreateIndex("dbo.UserActivities", "ItemId");
            AddForeignKey("dbo.UserActivities", "ItemId", "dbo.Items", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserActivities", "ItemId", "dbo.Items");
            DropIndex("dbo.UserActivities", new[] { "ItemId" });
            DropColumn("dbo.UserActivities", "ItemId");
            DropColumn("dbo.UserActivities", "Transaction");
            DropColumn("dbo.UserActivities", "Remarks");
            DropColumn("dbo.UserActivities", "CurrentStock");
            DropColumn("dbo.UserActivities", "Amount");
            DropColumn("dbo.UserActivities", "Quantity");
            DropColumn("dbo.UserActivities", "ReferenceNumber");
        }
    }
}
