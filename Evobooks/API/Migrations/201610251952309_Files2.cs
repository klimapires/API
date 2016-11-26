namespace API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Files2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                        Files_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Files", t => t.Files_Id)
                .Index(t => t.Files_Id);
            
            AddColumn("dbo.Files", "Category_Id", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Files", "Category_Id");
            AddForeignKey("dbo.Files", "Category_Id", "dbo.Categories", "Id", cascadeDelete: true);
            DropColumn("dbo.Files", "FilePath");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Files", "FilePath", c => c.String(nullable: false));
            DropForeignKey("dbo.Tags", "Files_Id", "dbo.Files");
            DropForeignKey("dbo.Files", "Category_Id", "dbo.Categories");
            DropIndex("dbo.Tags", new[] { "Files_Id" });
            DropIndex("dbo.Files", new[] { "Category_Id" });
            DropColumn("dbo.Files", "Category_Id");
            DropTable("dbo.Tags");
            DropTable("dbo.Categories");
        }
    }
}
