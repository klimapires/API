namespace API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FileBinaryInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Files", "BinaryFile", c => c.Binary(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Files", "BinaryFile");
        }
    }
}
