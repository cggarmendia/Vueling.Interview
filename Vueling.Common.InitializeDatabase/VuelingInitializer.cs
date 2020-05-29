using System.Data.Entity;
using Vueling.DataAccess.EF.Context;
using Vueling.DataAccess.EF.Migrations;

namespace Vueling.Common.InitializeDatabase
{
    public interface IVuelingInitializer
    {
        void InitializeDatabase();
    }

    public class VuelingInitializer : IVuelingInitializer
    {
        public void InitializeDatabase()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<VuelingContext, VuelingConfiguration>());
        }
    }
}
