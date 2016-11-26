namespace API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class File : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Files", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Files", "Name");
        }
    }
}
