using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Extensibility;
using Sakuno.KanColle.Amatsukaze.Extensibility.Services;

using AutoSwitchING.Services;

namespace AutoSwitchING
{
    /// <summary>
    /// PreferencePaneView.xaml 的交互逻辑
    /// </summary>
    public partial class PreferencePaneView : UserControl
    {
        public PreferencePaneView()
        {
            InitializeComponent();
        }
        
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            ServiceManager.GetService<ISwitchSettingsService>().AddSwitchSetting(string.Empty, 0, false);
        }

        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            var setting = SettingsList.SelectedItem as SwitchSetting;
            if (setting != null)
            {
                AutoSwitch.CurrentSettings.Values.Remove(setting);
            }
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            SettingsList.SelectedIndex = -1;
            var sss = ServiceManager.GetService<ISwitchSettingsService>();
            if (sss != null)
            {
                sss.UpdateSubscription();
                sss.SaveSwitchSettings();
            }
        }
    }
}
