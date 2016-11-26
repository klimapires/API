namespace API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserDetails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Country", c => c.String());
            AddColumn("dbo.AspNetUsers", "City", c => c.String());
            AddColumn("dbo.AspNetUsers", "Class", c => c.String());
            AddColumn("dbo.AspNetUsers", "Formation", c => c.String());
            AddColumn("dbo.AspNetUsers", "WorkInstitution", c => c.String());
            AddColumn("dbo.AspNetUsers", "FormationInstitution", c => c.String());
            AddColumn("dbo.AspNetUsers", "FormationCourse", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "FormationCourse");
            DropColumn("dbo.AspNetUsers", "FormationInstitution");
            DropColumn("dbo.AspNetUsers", "WorkInstitution");
            DropColumn("dbo.AspNetUsers", "Formation");
            DropColumn("dbo.AspNetUsers", "Class");
            DropColumn("dbo.AspNetUsers", "City");
            DropColumn("dbo.AspNetUsers", "Country");
        }
    }
}
