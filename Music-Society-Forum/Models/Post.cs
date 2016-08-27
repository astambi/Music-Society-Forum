using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Music_Society_Forum.Models
{
    public class Post
    {

        public Post()
        {
            this.Date = DateTime.Now;            
            this.Comments = new HashSet<Comment>(); // added Comments in Post class
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Body { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public string Author_Id { get; set; }

        [ForeignKey("Author_Id")]
        public ApplicationUser Author { get; set; }

        // added Comments in Post class
        public virtual ICollection<Comment> Comments { get; set; }

    }
}