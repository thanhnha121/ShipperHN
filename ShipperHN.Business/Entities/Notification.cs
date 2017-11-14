using System;

namespace ShipperHN.Business.Entities
{
    public class Notification : ShipperHNAbstract
    {
        public User UserComment { get; set; }

        public DateTime Time { get; set; }

        public Post Post { get; set; }
    }
}
