namespace API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class xap : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ImageContents", "Image", c => c.Binary());
            DropColumn("dbo.ImageContents", "ImagePath");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ImageContents", "ImagePath", c => c.String());
            DropColumn("dbo.ImageContents", "Image");
        }
    }
}
