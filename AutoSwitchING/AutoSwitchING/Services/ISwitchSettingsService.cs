using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSwitchING.Services
{
    interface ISwitchSettingsService
    {
        void AddSwitchSetting(string api, int index, bool isEnabled = true);
        void SaveSwitchSettings();
        void UpdateSubscription();
        Lazy<object> ToolPaneView { get; }
        SwitchSettings Settings { get; }
    }
}
