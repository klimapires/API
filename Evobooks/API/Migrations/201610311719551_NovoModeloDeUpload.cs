namespace API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NovoModeloDeUpload : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Files", "PreviewImage");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Files", "PreviewImage", c => c.Binary());
        }
    }
}
