namespace InventoryServices.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateItemincludeaddSupplier : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Items", "SupplierId", c => c.Int());
            CreateIndex("dbo.Items", "SupplierId");
            AddForeignKey("dbo.Items", "SupplierId", "dbo.Suppliers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Items", "SupplierId", "dbo.Suppliers");
            DropIndex("dbo.Items", new[] { "SupplierId" });
            DropColumn("dbo.Items", "SupplierId");
        }
    }
}
