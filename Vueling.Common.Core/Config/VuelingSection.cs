using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vueling.Common.Core.Config
{
    public static class VuelingAppSettings
    {
        #region Properties
        public static string LogLevel => ConfigurationManager.AppSettings["LogLevel"];
        public static string LogsPath => ConfigurationManager.AppSettings["LogsPath"];
        #endregion Properties
    }
}
