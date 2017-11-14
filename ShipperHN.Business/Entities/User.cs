using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShipperHN.Business.Entities
{
    public class User : ShipperHNAbstract
    {
        [Key, MaxLength(30), Required, Column(TypeName = "VARCHAR")]
        public string Id { get; set; }

        [MaxLength(256), Required, Column(TypeName = "NVARCHAR")]
        public string Name { get; set; }

        [MaxLength(256), Required, Column(TypeName = "VARCHAR")]
        public string AvataLink { get; set; }

        [Required]
        public DateTime LatestViewNotificationTime { get; set; }

        [MinLength(6), MaxLength(12), Column(TypeName = "VARCHAR")]
        public string Password { get; set; }

        [NotMapped]
        List<Notification> Notifications { get; set; } 
            
        [MinLength(6), MaxLength(12), Column(TypeName = "VARCHAR")]
        public string Username { get; set; }

        public List<PhoneNumber> PhoneNumbers { get; set; }

        public User()
        {
            PhoneNumbers = new List<PhoneNumber>();
            Notifications = new List<Notification>();
        }
    }
}
