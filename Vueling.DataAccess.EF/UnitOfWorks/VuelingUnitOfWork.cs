using Vueling.Common.DataAccess.EF.UnitOfWork;
using Vueling.DataAccess.EF.Context;

namespace Vueling.DataAccess.EF.UnitOfWorks
{
    public class VuelingUnitOfWork : UnitOfWorkBase
    {
        #region Constructor

        public VuelingUnitOfWork()
        {
            this.context = new VuelingContext();
        }

        #endregion
    }
}
