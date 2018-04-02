using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShipperHN.Business.Entities
{
    public class Location
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(20), Column(TypeName = "NVARCHAR")]
        public string Name { get; set; }
        [MaxLength(255), Column(TypeName = "NVARCHAR")]
        public string Streets { get; set; }

        public List<Post> Posts { get; set; }

        public DateTime CreatedTime { get; set; }

        public Location()
        {
            Posts = new List<Post>();
            Name = String.Empty;
            Streets = String.Empty;
            CreatedTime = DateTime.Now;
        }
    }
}
