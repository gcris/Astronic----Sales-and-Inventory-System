namespace InventoryServices.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSalesReturn : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SalesReturnDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remarks = c.String(),
                        SalesInvoiceDetailId = c.Int(nullable: false),
                        SalesReturnId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SalesInvoiceDetails", t => t.SalesInvoiceDetailId)
                .ForeignKey("dbo.SalesReturns", t => t.SalesReturnId, cascadeDelete: true)
                .Index(t => t.SalesInvoiceDetailId)
                .Index(t => t.SalesReturnId);
            
            CreateTable(
                "dbo.SalesReturns",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        TotalQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReferenceNumber = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SalesReturnDetails", "SalesReturnId", "dbo.SalesReturns");
            DropForeignKey("dbo.SalesReturnDetails", "SalesInvoiceDetailId", "dbo.SalesInvoiceDetails");
            DropIndex("dbo.SalesReturnDetails", new[] { "SalesReturnId" });
            DropIndex("dbo.SalesReturnDetails", new[] { "SalesInvoiceDetailId" });
            DropTable("dbo.SalesReturns");
            DropTable("dbo.SalesReturnDetails");
        }
    }
}
