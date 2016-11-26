namespace API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserDetails1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Course", c => c.String());
            DropColumn("dbo.AspNetUsers", "Class");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Class", c => c.String());
            DropColumn("dbo.AspNetUsers", "Course");
        }
    }
}
