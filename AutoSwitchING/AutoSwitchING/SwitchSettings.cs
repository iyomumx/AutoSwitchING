using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sakuno.KanColle.Amatsukaze.Models.Preferences;
using Sakuno.KanColle.Amatsukaze;

namespace AutoSwitchING
{
    public class SwitchSettings
    {
        public ObservableCollection<SwitchSetting> Values { get; private set; }

        public SwitchSettings(SwitchSetting[] prop = null)
        {
            Values = new ObservableCollection<SwitchSetting>(prop);
        }
    }
}
