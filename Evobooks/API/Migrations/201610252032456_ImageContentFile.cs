namespace API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ImageContentFile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ImageContents", "File_Id", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.ImageContents", "File_Id");
            AddForeignKey("dbo.ImageContents", "File_Id", "dbo.Files", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ImageContents", "File_Id", "dbo.Files");
            DropIndex("dbo.ImageContents", new[] { "File_Id" });
            DropColumn("dbo.ImageContents", "File_Id");
        }
    }
}
