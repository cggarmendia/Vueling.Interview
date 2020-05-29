using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Vueling.Common.Core.Config;
using Vueling.Common.Core.Log;
using Vueling.Presentation.Api.App_Start;

namespace Vueling.Presentation.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            NinjectWebCommon.RegisterNinject(GlobalConfiguration.Configuration);
            InitLog();
        }

        private static void InitLog()
        {
            var logPath = VuelingAppSettings.LogsPath;
            var logLevel = Logger.ParseLogLevel(VuelingAppSettings.LogLevel);
            if (!Directory.Exists(logPath)) Directory.CreateDirectory(logPath);

            Logger.Init(logPath, "Vueling", logLevel);
            Logger.AddLOGMsg("Log Inicializado");
        }
    }
}
