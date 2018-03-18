using System.Data.Entity.Migrations;
using System.Linq;
using ShipperHN.Business.Entities;

namespace ShipperHN.Business.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ShipperHNDBcontext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(ShipperHNDBcontext context)
        {
            ShipperHNAbstract hnAbstract = new User();
            string[] listLocations = hnAbstract.GetLocations();
            foreach (string location in listLocations)
            {
                if (context.Locations.FirstOrDefault(x => x.Title.Equals(location)) == null)
                {
                    context.Locations.Add(new Location
                    {
                        Title = location
                    });
                }
            }
            context.SaveChanges();
        }

        public static void Main(string[] args)
        {
            new Configuration().Seed(new ShipperHNDBcontext());
        }
    }
}
