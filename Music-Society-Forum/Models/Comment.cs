using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Music_Society_Forum.Models
{
    public class Comment
    {
        public Comment()
        {
            this.Date = DateTime.Now;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public String Text { get; set; }

        [Required]
        public DateTime Date { get; set; }

        //public int Post_Id { get; set; }
        //[ForeignKey("Post_Id")]
        [Required]
        public Post Post { get; set; }

        public string Author_Id { get; set; }
        [ForeignKey("Author_Id")]
        [Required]
        public ApplicationUser Author { get; set; }
    }
}