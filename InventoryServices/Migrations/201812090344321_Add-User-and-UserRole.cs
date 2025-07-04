namespace InventoryServices.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserandUserRole : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PasswordHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Password = c.String(),
                        Date = c.DateTime(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(),
                        Password = c.String(),
                        LastName = c.String(),
                        FirstName = c.String(),
                        Address = c.String(),
                        ContactNumber = c.String(),
                        IsEnable = c.Boolean(nullable: false),
                        UserRoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserRoles", t => t.UserRoleId)
                .Index(t => t.UserRoleId);
            
            CreateTable(
                "dbo.UserRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoleName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserPrivileges",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Action = c.String(),
                        IsEnable = c.Boolean(nullable: false),
                        UserRoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserRoles", t => t.UserRoleId)
                .Index(t => t.UserRoleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "UserRoleId", "dbo.UserRoles");
            DropForeignKey("dbo.UserPrivileges", "UserRoleId", "dbo.UserRoles");
            DropForeignKey("dbo.PasswordHistories", "UserId", "dbo.Users");
            DropIndex("dbo.UserPrivileges", new[] { "UserRoleId" });
            DropIndex("dbo.Users", new[] { "UserRoleId" });
            DropIndex("dbo.PasswordHistories", new[] { "UserId" });
            DropTable("dbo.UserPrivileges");
            DropTable("dbo.UserRoles");
            DropTable("dbo.Users");
            DropTable("dbo.PasswordHistories");
        }
    }
}
