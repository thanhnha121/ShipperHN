using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShipperHN.Business.Entities
{
    public class User : ShipperHNAbstract
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(30), Required, Column(TypeName = "VARCHAR")]
        public string UserId { get; set; }

        [MaxLength(255), Required, Column(TypeName = "NVARCHAR")]
        public string Name { get; set; }

        [Required]
        public DateTime LatestViewNotificationTime { get; set; }

        [MaxLength(255), Column(TypeName = "VARCHAR")]
        public string Password { get; set; }

        [NotMapped]
        List<Notification> Notifications { get; set; } 
            
        [MinLength(6), MaxLength(16), Column(TypeName = "VARCHAR")]
        public string Username { get; set; }

        public List<PhoneNumber> PhoneNumbers { get; set; }

        public User()
        {
            PhoneNumbers = new List<PhoneNumber>();
            Notifications = new List<Notification>();
        }
    }
}
