using Vueling.Common.Core.Context;
using Vueling.Common.Core.IoC;
using Ninject.Modules;
using Vueling.Common.Core.Key;

namespace Vueling.Common.Core.Config
{
    public static class AppConfig
    {
        #region Public Methods

        public static string GetUserName()
        {
            ICallContext context = DependencyManager.Instance().Resolve<ICallContext>();
            return context.Retrieve<string>(AppKeyConst.CurrentUser, string.Empty);
        }

        public static void InitIoC(INinjectModule[] ninjectModuleConfig)
        {
            DependencyManager.Instance().Configure(ninjectModuleConfig);
        }

        public static void SetUserName(string userName)
        {
            ICallContext context = DependencyManager.Instance().Resolve<ICallContext>();
            context.Save(AppKeyConst.CurrentUser, userName);
        }

        #endregion
    }
}
