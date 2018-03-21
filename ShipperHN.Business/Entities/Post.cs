using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShipperHN.Business.Entities
{
    public class Post : ShipperHNAbstract
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public User User { get; set; }

        [MaxLength(35), Column(TypeName = "VARCHAR")]
        public string PostId { get; set; }

        [Required, Column(TypeName = "NVARCHAR")]
        public string Message { get; set; }

        [Required]
        public DateTime CreatedTime { get; set; }

        [Required]
        public DateTime InsertTime { get; set; }

        public List<Comment> Comments { get; set; }

        [MaxLength(512), Column(TypeName = "VARCHAR")]
        public string FullPicture { get; set; }

        public List<Location> Locations { get; set; }

        public Post()
        {
            Comments = new List<Comment>();
            FullPicture = String.Empty;
            Locations = new List<Location>();
            InsertTime = DateTime.Now;
        }

    }
}
