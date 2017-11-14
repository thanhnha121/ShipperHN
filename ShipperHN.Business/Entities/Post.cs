using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShipperHN.Business.Entities
{
    public class Post : ShipperHNAbstract
    {
        [Required]
        public User User { get; set; }

        [Key, MaxLength(35), Column(TypeName = "VARCHAR")]
        public string Idpost { get; set; }

        [Required, MaxLength(1000), Column(TypeName = "NVARCHAR")]
        public string Message { get; set; }

        [Required]
        public DateTime CreatedTime { get; set; }

        [Required]
        public DateTime InsertTime { get; set; }

        public List<Comment> Comments { get; set; }

        [MaxLength(512), Column(TypeName = "VARCHAR")]
        public string FullPicture { get; set; }

        [MaxLength(512), Column(TypeName = "NVARCHAR")]
        public string Places { get; set; }

        public Post()
        {
            Comments = new List<Comment>();
            FullPicture = String.Empty;
        }

    }
}
