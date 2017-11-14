using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShipperHN.Business.Entities
{
    public class Comment : ShipperHNAbstract
    {
        [Key, Required]
        public User User { get; set; }

        [Key, Required]
        public DateTime Time { get; set; }

        [Required, MaxLength(200), Column(TypeName = "NVARCHAR")]
        public string Message { get; set; }

        [Required, MaxLength(10), Column(TypeName = "VARCHAR")]
        public string Type { get; set; }
    }
}
