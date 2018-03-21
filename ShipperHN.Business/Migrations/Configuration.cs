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
        }

        public static void Main(string[] args)
        {
            new Configuration().Seed(new ShipperHNDBcontext());
        }
    }
}
