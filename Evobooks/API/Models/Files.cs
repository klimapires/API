using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using API.Models;

namespace API.Models
{
    public class Files
    {
        [Required]
        [Key]
        public string Id { get; set; }


        [Required]
        public string Name { get; set; }

        [Required]
        public virtual Categories Category { get; set; }

        [Required]
        public DateTime UpdateDateTime { get; set; }

        [Required]
        public byte[] FinalInformation { get; set; }


        [Required]
        public byte[] BinaryFile { get; set; }

        [Required]
        public virtual ApplicationUser ApplicationUser { get; set; }

        
    }

    public class FileTag
    {
        [Key, ForeignKey("Tag"), Column(Order =1)]
        public string Tag_Id { get; set; }
        
        public virtual Tags Tag { get; set; }

        [Key, ForeignKey("File"), Column(Order = 0)]
        public string File_Id { get; set; }

        public virtual Files File { get; set; }
    }
    public class Categories
    {
        [Required]
        [Key]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }
    }

    public class Tags
    {
        [Required]
        [Key]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }
    }

    public class ImageContents
    {
        [Required]
        [Key]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        public byte[] Image { get; set; }

        [Required]
        public virtual ContentType ContentType { get; set; }    

        [Required]
        public virtual Files File { get; set; }
        
    }

    public class ContentType
    {
        [Required]
        [Key]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

    }



}