using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShipperHN.Business.Entities
{
    public class User : ShipperHNAbstract
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(30), Column(TypeName = "VARCHAR")]
        public string UserId { get; set; }

        [MaxLength(255), Column(TypeName = "VARCHAR")]
        public string UserProfileUrl { get; set; }

        [MaxLength(255), Column(TypeName = "VARCHAR")]
        public string UserProfilePicture{ get; set; }

        [MaxLength(255), Column(TypeName = "NVARCHAR")]
        public string Name { get; set; }

        [Required]
        public DateTime LatestViewNotificationTime { get; set; }

        [MaxLength(255), Column(TypeName = "VARCHAR")]
        public string Password { get; set; }

        [NotMapped]
        List<Notification> Notifications { get; set; } 
            
        [MaxLength(16), Column(TypeName = "VARCHAR")]
        public string Username { get; set; }

        public List<PhoneNumber> PhoneNumbers { get; set; }

        [Required]
        public DateTime InsertTime { get; set; }

        public User()
        {
            PhoneNumbers = new List<PhoneNumber>();
            Notifications = new List<Notification>();
            LatestViewNotificationTime = DateTime.Now;
            InsertTime = DateTime.Now;
        }
    }
}
