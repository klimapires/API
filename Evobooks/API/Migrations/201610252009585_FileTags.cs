namespace API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FileTags : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tags", "Files_Id", "dbo.Files");
            DropIndex("dbo.Tags", new[] { "Files_Id" });
            CreateTable(
                "dbo.ContentTypes",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ImageContents",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                        ImagePath = c.String(),
                        ContentType_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ContentTypes", t => t.ContentType_Id)
                .Index(t => t.ContentType_Id);
            
            DropColumn("dbo.Tags", "Files_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tags", "Files_Id", c => c.String(maxLength: 128));
            DropForeignKey("dbo.ImageContents", "ContentType_Id", "dbo.ContentTypes");
            DropIndex("dbo.ImageContents", new[] { "ContentType_Id" });
            DropTable("dbo.ImageContents");
            DropTable("dbo.ContentTypes");
            CreateIndex("dbo.Tags", "Files_Id");
            AddForeignKey("dbo.Tags", "Files_Id", "dbo.Files", "Id");
        }
    }
}
