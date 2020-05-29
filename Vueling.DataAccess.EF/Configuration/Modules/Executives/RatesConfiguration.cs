using Vueling.Common.DataAccess.EF.Configuration;
using Vueling.Common.DataAccess.EF.Extensions;
using Vueling.Domain.Entities.Modules.Executives;

namespace Vueling.DataAccess.EF.Configuration.Modules.Executives
{
    public class RatesConfiguration : DbContextBaseConfiguration<Rates>
    {
        #region Ctor.
        public RatesConfiguration()
            : base()
        {
            ToTable("Rates");
            HasKey(r => new { r.From, r.To});
        }
        #endregion
    }
}
