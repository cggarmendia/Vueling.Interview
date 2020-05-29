using Vueling.Common.Core.Context;
using Vueling.Common.DataAccess.EF.UnitOfWork;
using Vueling.Common.InitializeDatabase;
using Vueling.DataAccess.EF.UnitOfWorks;
using Vueling.Domain.UnitOfWork;
using Ninject.Modules;
using Vueling.Business.DomainServices.Modules.Executive;
using Vueling.Business.HttpClientHelper;
using Vueling.Common.Presentation.API;
using Vueling.Common.DataAccess.Contracts;
using Vueling.Common.DataAccess.Implementations;

namespace Vueling.Common.IoC
{
    public class IoCConfigModule : NinjectModule
    {
        #region Public Methods

        public override void Load()
        {
            // Infraestructure
            Bind<IUnitOfWork>().To<VuelingUnitOfWork>();
            Bind<IUnitOfWorkManager>().To<UnitOfWorkManager>().InSingletonScope();

            // Call Context
            Bind<ICallContext>().To<WebCallContext>().InSingletonScope();

            // DomainServices
            Bind<IRatesDomainServices>().To<RatesDomainServices>().InSingletonScope();
            Bind<ITransactionsDomainServices>().To<TransactionsDomainServices>().InSingletonScope();

            // HttpClientHelper
            Bind<IVuelingHttpClientHelper>().To<VuelingHttpClientHelper>().InSingletonScope();

            // InitializeDatabase
            Bind<IVuelingInitializer>().To<VuelingInitializer>().InSingletonScope();

            // SqlBulkLogic
            Bind<ISqlBulkLogic>().To<SqlBulkLogic>().InSingletonScope();
        }

        #endregion Public Methods
    }
}
