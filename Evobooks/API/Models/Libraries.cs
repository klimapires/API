using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class Libraries
    {
        [Key,Required]
        public string ID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public virtual ApplicationUser Creator { get; set; }

        [Required]
        public DateTime CreationDateTime { get; set; }

    }

    public class LibrariesUsers
    {
        [Key, Column(Order =0),Required, ForeignKey("User")]
        public string User_Id { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Key, Column(Order = 1), Required, ForeignKey("Library")]
        public string  Library_Id { get; set; }
        public virtual Libraries Library { get; set; }
    }

    public class LibrariesFiles
    {
        [Key, Column(Order = 0), Required, ForeignKey("File")]
        public string File_Id { get; set; }
        public virtual Files File { get; set; }


        [Key, Column(Order = 1), Required, ForeignKey("Library")]
        public string Library_Id { get; set; }
        public virtual Libraries Library { get; set; }
    }
}

