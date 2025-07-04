namespace InventoryServices.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateItemsincludeOtherPartNo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Items", "OtherPartNo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Items", "OtherPartNo");
        }
    }
}
