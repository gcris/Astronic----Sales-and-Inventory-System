namespace InventoryServices.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateItemPrice : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ItemPrices", "ItemPrice_Id", "dbo.ItemPrices");
            DropIndex("dbo.ItemPrices", new[] { "ItemPrice_Id" });
            DropColumn("dbo.ItemPrices", "ItemPrice_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ItemPrices", "ItemPrice_Id", c => c.Int());
            CreateIndex("dbo.ItemPrices", "ItemPrice_Id");
            AddForeignKey("dbo.ItemPrices", "ItemPrice_Id", "dbo.ItemPrices", "Id");
        }
    }
}
