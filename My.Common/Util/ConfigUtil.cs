using My.Common.Util.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.Common.Util
{
    public class ConfigUtil
    {
        private static IConfig configStatic;

        public static IConfig Config
        {
            get
            {
                return ConfigUtil.configStatic;
            }
            set
            {
                if (ConfigUtil.configStatic != value)
                {
                    if (ConfigUtil.configStatic != null)
                    {
                        throw new Exception("Config service must only be set once.");
                    }
                    ConfigUtil.configStatic = value;
                }
            }
        }

        public static string GetConfigurationSetting(string configurationSetting, string defaultValue, bool throwIfNull)
        {
            if (ConfigUtil.Config == null)
            {
                throw new Exception("Config service not initialized");
            }
            try
            {
                return ConfigUtil.Config.GetConfigurationSetting(configurationSetting, defaultValue, throwIfNull);
            }
            catch (Exception ex)
            {
                LogUtil.LogError("My.Common.Util.ConfigUtil: " + ex.Message, ex.StackTrace, 0L);
                throw;
            }
        }

        public static string GetConfigItem(string name)
        {
            if (ConfigUtil.Config == null)
            {
                throw new Exception("Config service not initialized");
            }
            try
            {
                return ConfigUtil.Config.GetConfigItem(name);
            }
            catch (Exception ex)
            {
                LogUtil.LogError(string.Format("My.Common.Util.ConfigUtil ({0}): {1}", name, ex.Message), ex.StackTrace, 0L);
                throw;
            }
        }
    }
}
