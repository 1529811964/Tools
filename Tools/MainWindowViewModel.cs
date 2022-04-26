using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Tools_IPlugin;

namespace Tools
{
    public class MainWindowViewModel
    {
        public ObservableCollection<IPlugin> Plugins { get; set; } = new ObservableCollection<IPlugin>();
        public ICommand AddCommand { get; set; }
        public ICommand RemoveCommand { get; set; }

        public MainWindowViewModel()
        {
            Plugin_info plugin_Info = new Plugin_info("Plugin1", "1.0.0", "wisokey", DateTime.Now.Date, true);
            plugin_Info.Settings.Add(new Tools_IPlugin.Setting("Key_String", SettingType.String, "value_String"));
            Plugins.Add(plugin_Info);
            plugin_Info = new Plugin_info("Plugin2", "2.0.0", "wisokey", DateTime.Now.AddDays(-1).Date, false);
            plugin_Info.Settings.Add(new Tools_IPlugin.Setting("Key_Switch", SettingType.Switch, "True"));
            Plugins.Add(plugin_Info);
            plugin_Info = new Plugin_info("Plugin3", "3.0.0", "洛基", DateTime.Now.AddDays(-2).Date, true);
            plugin_Info.Settings.Add(new Tools_IPlugin.Setting("Key_Hotkey", SettingType.Hotkey, "value_Hotkey"));
            Plugins.Add(plugin_Info);

            //AddCommand = new RelayCommand(Add);
            //RemoveCommand = new RelayCommand(Remove);
        }

        public void Add()
        {
            Plugins.Add(new Plugin_info("Plugin3", "3.0.0", "洛基", DateTime.Now.AddDays(-2).Date, true));
        }

        public void Remove()
        {
            if (Plugins.Count > 0)
            {
                Plugins.RemoveAt(Plugins.Count - 1);
            }
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
            this.Active = Active.ToString();
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
