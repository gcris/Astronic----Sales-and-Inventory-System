namespace InventoryServices.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateCaterogyaddLastUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Categories", "LastUpdate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Categories", "LastUpdate");
        }
    }
}
