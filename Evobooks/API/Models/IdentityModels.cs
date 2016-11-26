using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace API.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
        public string Country { get; set; }
        public string City { get; set; }

        public string Course { get; set; }

        public string Formation { get; set; }

        public string WorkInstitution { get; set; }

        public string FormationInstitution { get; set; }

        public string FormationCourse { get; set; }
    }


    
    public class DBContext : IdentityDbContext<ApplicationUser>
    {
        public DBContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
        }
        public static DBContext Create()
        {
            return new DBContext();
        }

        public System.Data.Entity.DbSet<API.Models.Files> Files { get; set; }
        public System.Data.Entity.DbSet<API.Models.Categories> Categories { get; set; }
        public System.Data.Entity.DbSet<API.Models.ImageContents> ImageContents { get; set; }

        public System.Data.Entity.DbSet<API.Models.ContentType> ContentType { get; set; }

        public System.Data.Entity.DbSet<API.Models.Tags> Tags { get; set; }

        public System.Data.Entity.DbSet<API.Models.FileTag> FileTag { get; set; }

        public System.Data.Entity.DbSet<API.Models.Libraries> Libraries { get; set; }

        public System.Data.Entity.DbSet<API.Models.LibrariesUsers> LibrariesUsers { get; set; }
        public System.Data.Entity.DbSet<API.Models.LibrariesFiles> LibrariesFiles { get; set; }
        //public System.Data.Entity.DbSet<API.Models.ApplicationUser> ApplicationUsers { get; set; }
    }
}