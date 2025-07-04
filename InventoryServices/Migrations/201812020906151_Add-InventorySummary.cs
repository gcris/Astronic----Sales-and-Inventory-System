namespace InventoryServices.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInventorySummary : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InventorySummaries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        TotalSold = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalSalesReturn = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPurchase = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPurchaseReturn = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.InventorySummaryDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BeginningInv = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PurchasedItems = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SoldItems = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SalesReturnItems = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PurchaseOrderReturnItems = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EndingInv = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ItemId = c.Int(nullable: false),
                        InventorySummaryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.InventorySummaries", t => t.InventorySummaryId, cascadeDelete: true)
                .ForeignKey("dbo.Items", t => t.ItemId, cascadeDelete: true)
                .Index(t => t.ItemId)
                .Index(t => t.InventorySummaryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InventorySummaryDetails", "ItemId", "dbo.Items");
            DropForeignKey("dbo.InventorySummaryDetails", "InventorySummaryId", "dbo.InventorySummaries");
            DropIndex("dbo.InventorySummaryDetails", new[] { "InventorySummaryId" });
            DropIndex("dbo.InventorySummaryDetails", new[] { "ItemId" });
            DropTable("dbo.InventorySummaryDetails");
            DropTable("dbo.InventorySummaries");
        }
    }
}
