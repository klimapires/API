namespace API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Files1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Files", "PreviewImage", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Files", "PreviewImage");
        }
    }
}
