using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.Common.Util.Config
{
    public interface IConfig
    {
        string GetConfigItem(string name);
        string GetConfigurationSetting(string configurationSetting, string defaultValue, bool throwIfNull);
    }
}
