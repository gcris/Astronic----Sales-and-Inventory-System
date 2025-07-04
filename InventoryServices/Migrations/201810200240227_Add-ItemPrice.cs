namespace InventoryServices.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddItemPrice : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ItemPrices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Price1 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Price2 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CurrentCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LastUpdate = c.DateTime(nullable: false),
                        ItemId = c.Int(nullable: false),
                        ItemPrice_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Items", t => t.ItemId, cascadeDelete: true)
                .ForeignKey("dbo.ItemPrices", t => t.ItemPrice_Id)
                .Index(t => t.ItemId)
                .Index(t => t.ItemPrice_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemPrices", "ItemPrice_Id", "dbo.ItemPrices");
            DropForeignKey("dbo.ItemPrices", "ItemId", "dbo.Items");
            DropIndex("dbo.ItemPrices", new[] { "ItemPrice_Id" });
            DropIndex("dbo.ItemPrices", new[] { "ItemId" });
            DropTable("dbo.ItemPrices");
        }
    }
}
