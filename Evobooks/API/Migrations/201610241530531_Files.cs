namespace API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Files : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Files",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FilePath = c.String(nullable: false),
                        UpdateDateTime = c.DateTime(nullable: false),
                        FinalInformation = c.Binary(nullable: false),
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .Index(t => t.ApplicationUser_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Files", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Files", new[] { "ApplicationUser_Id" });
            DropTable("dbo.Files");
        }
    }
}
