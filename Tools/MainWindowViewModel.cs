using System;
using System.Collections.ObjectModel;
using Tools_IPlugin;

namespace Tools
{
    public class MainWindowViewModel
    {
        public static MainWindowViewModel ViewModel { get; private set; } = new MainWindowViewModel();

        public ObservableCollection<ModPlugin> Plugins { get; set; } = new ObservableCollection<ModPlugin>();

        public static MainWindowViewModel GetInstance()
        {
            if (ViewModel == null)
            {
                ViewModel = new MainWindowViewModel();
            }
            return ViewModel;
        }

        private MainWindowViewModel()
        {
            Plugin_info plugin_Info = new Plugin_info("Plugin1", "1.0.0", "wisokey", DateTime.Now.Date, true);
            plugin_Info.Settings.Add(new Tools_IPlugin.Setting("Key_String", SettingType.String, "value_String"));
            Plugins.Add(new ModPlugin(plugin_Info, "1.dll"));
            plugin_Info = new Plugin_info("Plugin2", "2.0.0", "wisokey", DateTime.Now.AddDays(-1).Date, false);
            plugin_Info.Settings.Add(new Tools_IPlugin.Setting("Key_Switch", SettingType.Switch, "True"));
            Plugins.Add(new ModPlugin(plugin_Info, "2.dll"));
            plugin_Info = new Plugin_info("Plugin3", "3.0.0", "洛基", DateTime.Now.AddDays(-2).Date, true);
            plugin_Info.Settings.Add(new Tools_IPlugin.Setting("Key_Hotkey", SettingType.Hotkey, "value_Hotkey"));
            Plugins.Add(new ModPlugin(plugin_Info, "3.dll"));
        }
    }

    public class Plugin_info : IPlugin
    {
        public Plugin_info(string Name, string Version, string Author, DateTime LastTime, bool Active)
        {
            this.Name = Name;
            this.Version = Version;
            this.Author = Author;
            this.LastTime = LastTime;
            this.Active = Active;
        }

        public override void Init()
        {
            throw new NotImplementedException();
        }

        public override void Disabled()
        {
            throw new NotImplementedException();
        }

        public override void Hotkeys(string code)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
