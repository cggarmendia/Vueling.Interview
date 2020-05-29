using Vueling.Common.Core.Context;
using Vueling.Common.Core.IoC;
using Vueling.Common.Core.Key;
using Vueling.Domain.UnitOfWork;

namespace Vueling.Common.DataAccess.EF.UnitOfWork
{

    public class UnitOfWorkManager : IUnitOfWorkManager
    {
        #region Members

        private ICallContext callContext;
        private object lockUoW = new object();

        #endregion Members

        #region Constructor

        public UnitOfWorkManager(ICallContext context)
        {
            callContext = context;
        }

        #endregion Constructor

        #region Methods

        public virtual IUnitOfWork GetUoW()
        {
            if (!callContext.Contains(AppKeyConst.UoW))
            {
                lock (lockUoW)
                {
                    if (!callContext.Contains(AppKeyConst.UoW))
                    {
                        callContext.Save(AppKeyConst.UoW, DependencyManager.Instance().Resolve<IUnitOfWork>());
                    }
                }
            }

            return callContext.Retrieve<IUnitOfWork>(AppKeyConst.UoW);
        }

        #endregion Methods
    }
}
