namespace InventoryServices.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPurchaseOrderReturn : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PurchaseOrderReturnDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remarks = c.String(),
                        PurchaseOrderDetailId = c.Int(nullable: false),
                        PurchaseOrderReturnId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PurchaseOrderDetails", t => t.PurchaseOrderDetailId)
                .ForeignKey("dbo.PurchaseOrderReturns", t => t.PurchaseOrderReturnId, cascadeDelete: true)
                .Index(t => t.PurchaseOrderDetailId)
                .Index(t => t.PurchaseOrderReturnId);
            
            CreateTable(
                "dbo.PurchaseOrderReturns",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        TotalQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReferenceNumber = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.PurchaseOrders", "Returned", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PurchaseOrderReturnDetails", "PurchaseOrderReturnId", "dbo.PurchaseOrderReturns");
            DropForeignKey("dbo.PurchaseOrderReturnDetails", "PurchaseOrderDetailId", "dbo.PurchaseOrderDetails");
            DropIndex("dbo.PurchaseOrderReturnDetails", new[] { "PurchaseOrderReturnId" });
            DropIndex("dbo.PurchaseOrderReturnDetails", new[] { "PurchaseOrderDetailId" });
            DropColumn("dbo.PurchaseOrders", "Returned");
            DropTable("dbo.PurchaseOrderReturns");
            DropTable("dbo.PurchaseOrderReturnDetails");
        }
    }
}
