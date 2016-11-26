namespace API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FileTag : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FileTags",
                c => new
                    {
                        File_Id = c.String(nullable: false, maxLength: 128),
                        Tag_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.File_Id, t.Tag_Id })
                .ForeignKey("dbo.Files", t => t.File_Id, cascadeDelete: true)
                .ForeignKey("dbo.Tags", t => t.Tag_Id, cascadeDelete: true)
                .Index(t => t.File_Id)
                .Index(t => t.Tag_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FileTags", "Tag_Id", "dbo.Tags");
            DropForeignKey("dbo.FileTags", "File_Id", "dbo.Files");
            DropIndex("dbo.FileTags", new[] { "Tag_Id" });
            DropIndex("dbo.FileTags", new[] { "File_Id" });
            DropTable("dbo.FileTags");
        }
    }
}
