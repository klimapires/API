namespace API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CorrecaoChavesERequireds : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FileTags", "File_Id", "dbo.Files");
            DropForeignKey("dbo.FileTags", "Tag_Id", "dbo.Tags");
            DropForeignKey("dbo.ImageContents", "ContentType_Id", "dbo.ContentTypes");
            DropIndex("dbo.FileTags", new[] { "File_Id" });
            DropIndex("dbo.FileTags", new[] { "Tag_Id" });
            DropIndex("dbo.ImageContents", new[] { "ContentType_Id" });
            AlterColumn("dbo.ImageContents", "ContentType_Id", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.ImageContents", "ContentType_Id");
            AddForeignKey("dbo.ImageContents", "ContentType_Id", "dbo.ContentTypes", "Id", cascadeDelete: true);
            //DropTable("dbo.FileTags");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.FileTags",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        File_Id = c.String(maxLength: 128),
                        Tag_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.ImageContents", "ContentType_Id", "dbo.ContentTypes");
            DropIndex("dbo.ImageContents", new[] { "ContentType_Id" });
            AlterColumn("dbo.ImageContents", "ContentType_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.ImageContents", "ContentType_Id");
            CreateIndex("dbo.FileTags", "Tag_Id");
            CreateIndex("dbo.FileTags", "File_Id");
            AddForeignKey("dbo.ImageContents", "ContentType_Id", "dbo.ContentTypes", "Id");
            AddForeignKey("dbo.FileTags", "Tag_Id", "dbo.Tags", "Id");
            AddForeignKey("dbo.FileTags", "File_Id", "dbo.Files", "Id");
        }
    }
}
