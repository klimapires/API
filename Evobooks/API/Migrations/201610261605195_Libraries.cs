namespace API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Libraries : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Libraries",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                        CreationDateTime = c.DateTime(nullable: false),
                        Creator_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.Creator_Id, cascadeDelete: true)
                .Index(t => t.Creator_Id);
            
            CreateTable(
                "dbo.LibrariesFiles",
                c => new
                    {
                        File_Id = c.String(nullable: false, maxLength: 128),
                        Library_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.File_Id, t.Library_Id })
                .ForeignKey("dbo.Files", t => t.File_Id, cascadeDelete: true)
                .ForeignKey("dbo.Libraries", t => t.Library_Id, cascadeDelete: false)
                .Index(t => t.File_Id)
                .Index(t => t.Library_Id);
            
            CreateTable(
                "dbo.LibrariesUsers",
                c => new
                    {
                        User_Id = c.String(nullable: false, maxLength: 128),
                        Library_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.User_Id, t.Library_Id })
                .ForeignKey("dbo.Libraries", t => t.Library_Id, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: false)
                .Index(t => t.User_Id)
                .Index(t => t.Library_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LibrariesUsers", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.LibrariesUsers", "Library_Id", "dbo.Libraries");
            DropForeignKey("dbo.LibrariesFiles", "Library_Id", "dbo.Libraries");
            DropForeignKey("dbo.LibrariesFiles", "File_Id", "dbo.Files");
            DropForeignKey("dbo.Libraries", "Creator_Id", "dbo.AspNetUsers");
            DropIndex("dbo.LibrariesUsers", new[] { "Library_Id" });
            DropIndex("dbo.LibrariesUsers", new[] { "User_Id" });
            DropIndex("dbo.LibrariesFiles", new[] { "Library_Id" });
            DropIndex("dbo.LibrariesFiles", new[] { "File_Id" });
            DropIndex("dbo.Libraries", new[] { "Creator_Id" });
            DropTable("dbo.LibrariesUsers");
            DropTable("dbo.LibrariesFiles");
            DropTable("dbo.Libraries");
        }
    }
}
