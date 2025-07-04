namespace InventoryServices.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateCategoryandItemincludeMinimumStock : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Categories", "MinimumStock", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Items", "MinimumStock", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Items", "MinimumStock");
            DropColumn("dbo.Categories", "MinimumStock");
        }
    }
}
