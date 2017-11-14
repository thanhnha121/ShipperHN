using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShipperHN.Business.Entities
{
    public class PhoneNumber : ShipperHNAbstract
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(11), Column(TypeName = "VARCHAR")]
        public string Phone { get; set; }
    }
}
