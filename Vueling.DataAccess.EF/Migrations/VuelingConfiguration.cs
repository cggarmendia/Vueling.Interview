using System.Linq;

namespace Vueling.DataAccess.EF.Migrations
{
    using System.Data.Entity.Migrations;
    using Context;

    public sealed class VuelingConfiguration : DbMigrationsConfiguration<VuelingContext>
    {
        public VuelingConfiguration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(VuelingContext context)
        {
            if (!context.Rates.Any())
            {
                //ToDo: initial_data
            }
           
        }
    }
}
