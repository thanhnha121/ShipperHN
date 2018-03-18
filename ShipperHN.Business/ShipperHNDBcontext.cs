using System.Data.Entity;
using ShipperHN.Business.Entities;
using ShipperHN.Business.Migrations;

namespace ShipperHN.Business
{
    public class ShipperHNDBcontext : DbContext
    {

        public ShipperHNDBcontext () : base("name=connString")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ShipperHNDBcontext, Configuration>());
        }

        public DbSet<User> Users { get; set; } 
        public DbSet<Post> Posts { get; set; } 
        public DbSet<PhoneNumber> PhoneNumbers { get; set; } 
        public DbSet<Comment> Comments { get; set; } 
        public DbSet<Location> Locations { get; set; } 
    }
}
