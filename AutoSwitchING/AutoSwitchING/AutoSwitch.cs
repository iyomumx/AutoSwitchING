using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;

using Sakuno.KanColle.Amatsukaze.Extensibility;
using Sakuno.KanColle.Amatsukaze.Extensibility.Services;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Models.Preferences;
using Sakuno.KanColle.Amatsukaze.ViewModels;
using Sakuno.UserInterface.Controls;

using AutoSwitchING.Services;

namespace AutoSwitchING
{
    [PluginExport("AutoSwitchING", "iyomumx@NGA", "1.0.1", "5dff2c3d-6436-43d3-a590-8ddb278ee8c5")]
    [Export(typeof(IToolPane))]
    [Export(typeof(IToolPaneScrollBarVisibilities))]
    public class AutoSwitch : IPlugin, IToolPaneScrollBarVisibilities, IToolPane, ISwitchSettingsService
    {
        #region IToolPaneScrollBarVisibilities Implements

        ScrollBarVisibility IToolPaneScrollBarVisibilities.HorizontalScrollBarVisibility => ScrollBarVisibility.Disabled;
        ScrollBarVisibility IToolPaneScrollBarVisibilities.VerticalScrollBarVisibility => ScrollBarVisibility.Hidden;

        #endregion

        #region IToolPane Implements

        string IToolPane.Name => "自动切换";
        object IToolPane.View => ServiceManager.GetService<ISwitchSettingsService>().ToolPaneView;

        #endregion

        Lazy<PreferencePaneView> tabview = new Lazy<PreferencePaneView>(() =>
        {
            var view = new PreferencePaneView();
            view.SettingsList.SetBinding(FrameworkElement.DataContextProperty, new Binding("Settings") { Source = ServiceManager.GetService<ISwitchSettingsService>() });
            return view;
        });
        private IDisposable _subscription = null;
        private Visual mainwindow = null;
        private AdvancedTabControl tabc = null;

        private Property<SwitchSetting[]> SwitchSettingsProperty;
        private SwitchSettings _settings;
        public SwitchSettings Settings => _settings;

        public static SwitchSettings CurrentSettings => ServiceManager.GetService<ISwitchSettingsService>().Settings;

        /// <summary>
        /// This property is for design-time using only. Do not use it in any code except XAML tag d:DataContext.
        /// </summary>
        public static SwitchSettings DefaultSettings => new SwitchSettings(new SwitchSetting[]
            {
                new SwitchSetting() { Api="api_port/port", Index=0 },
                new SwitchSetting() { Api="api_req_sortie/battle", Index=1 },
                new SwitchSetting() { Api="api_req_practice/battle", Index=1 },
            });

        void IPlugin.Initialize()
        {
            SwitchSettingsProperty = new Property<SwitchSetting[]>("plugin.autoswitching.settings", new SwitchSetting[]
            {
                new SwitchSetting() { Api="api_port/port", Index=0 },
                new SwitchSetting() { Api="api_req_sortie/battle", SwitchBy = SwitchType.TabName, TabName = "出击" },
                new SwitchSetting() { Api="api_req_practice/battle", Index=1 },
            });
            _settings = new SwitchSettings(SwitchSettingsProperty);
            SwitchSettingsProperty.Subscribe(sss =>
            {
                _settings.Values.Clear();
                foreach (var s in sss)
                {
                    _settings.Values.Add(s);
                }
                DoUpdateSubscription();
            });

            ServiceManager.Register<ISwitchSettingsService>(this);
            DoUpdateSubscription();
        }

        void ISwitchSettingsService.AddSwitchSetting(string api, int index, bool isEnabled)
        {
            Settings.Values.Add(new SwitchSetting { Api = api, Index = index, IsEnabled = isEnabled });
        }

        void ISwitchSettingsService.SaveSwitchSettings()
        {
            SwitchSettingsProperty.Value = _settings.Values.ToArray();
        }

        void ISwitchSettingsService.UpdateSubscription()
        {
            DoUpdateSubscription();
        }

        object ISwitchSettingsService.ToolPaneView => tabview.Value;

        private void DoUpdateSubscription()
        {
            _subscription?.Dispose();
            var api_to_subscribe = Settings.Values?.Where(s => !string.IsNullOrEmpty(s.Api) && s.IsEnabled);
            if (api_to_subscribe != null && api_to_subscribe.Count() == 0) return;
            _subscription = ApiService.Subscribe(api_to_subscribe.Select(s => s.Api).ToArray(), apiinfo =>
            {
                // Don't dispose return object of HwndSource.FromHwnd, or it will close target window.
                // Suppose IMainWindowService has registered. Maybe a null-test is needed.
                if (mainwindow == null) mainwindow = HwndSource.FromHwnd(ServiceManager.GetService<IMainWindowService>().Handle).RootVisual;
                if (tabc == null) tabc = mainwindow?.Dispatcher.Invoke(() => FindChild<AdvancedTabControl>(mainwindow, null));
                var setting = Settings.Values.Where(s => s.Api == apiinfo.Api).SingleOrDefault();
                if (setting == null)
                {
                    return;
                }
                else if (setting.SwitchBy == SwitchType.Index)
                {
                    int index = setting.Index;
                    tabc?.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        tabc.SelectedIndex = index;
                    }));
                }
                else if (setting.SwitchBy == SwitchType.TabName)
                {
                    string name = setting.TabName;
                    tabc?.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        int? index = tabc.ItemsSource.OfType<TabItemViewModel>().Select((vm, i) => new { ViewModel = vm, Index = i }).Where(p => p.ViewModel.Name == name).FirstOrDefault()?.Index;
                        if (index.HasValue)
                        {
                            tabc.SelectedIndex = index.Value;
                        }
                    }));
                }
            });
        }

        #region WPF Helper
        // Source: http://stackoverflow.com/questions/636383/how-can-i-find-wpf-controls-by-name-or-type

        /// <summary>
        /// Finds a Child of a given item in the visual tree. 
        /// </summary>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="childName">x:Name or Name of child. </param>
        /// <returns>The first parent item that matches the submitted type parameter. 
        /// If not matching item can be found, 
        /// a null parent is being returned.</returns>
        public static T FindChild<T>(DependencyObject parent, string childName)
           where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }
        #endregion
    }
}
