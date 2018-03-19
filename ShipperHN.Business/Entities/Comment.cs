using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShipperHN.Business.Entities
{
    public class Comment : ShipperHNAbstract
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [MaxLength(35), Column(TypeName = "VARCHAR")]
        public string CommentId { get; set; }

        public User User { get; set; }

        [Required]
        public DateTime Time { get; set; }

        [Required, MaxLength(200), Column(TypeName = "NVARCHAR")]
        public string Message { get; set; }

        [Required, MaxLength(10), Column(TypeName = "VARCHAR")]
        public string Type { get; set; }
    }
}
