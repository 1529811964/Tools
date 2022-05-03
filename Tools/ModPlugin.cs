using System;
using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Tools_IPlugin;

namespace Tools
{
    public class ModPlugin : ObservableRecipient
    {
        private IPlugin plugin = null;
        /// <summary>
        /// 文件名称，不包含路径
        /// </summary>
        public string Filename { get; set; }

        public string Name => plugin?.Name;

        public string Version => plugin?.Version;

        public string Author => plugin?.Author;

        public DateTime LastTime => plugin.LastTime;

        public string Description => plugin?.Description;

        public bool Active
        {
            get => plugin.Active;
            set
            {
                if (plugin.Active != value)
                {
                    if (value)
                    {
                        Init();
                    }
                    else
                    {
                        Disabled();
                    }
                }
                plugin.Active = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Setting> Settings => plugin?.Settings;

        public ModPlugin(IPlugin plugin, string filename)
        {
            this.plugin = plugin;
            this.Filename = filename;
        }

        public void Disabled()
        {
            HotKeyManagement hotKeyManagement = HotKeyManagement.GetInstance();
            foreach (var setting in Settings)
            {
                if (setting.Type == SettingType.Hotkey)
                {
                    hotKeyManagement.UnregisterHotKey(setting.Value, this, setting.Key);
                }
            }
            plugin?.Disabled();
        }

        public void Dispose()
        {
            if (Active)
            {
                Disabled();
            }
            plugin?.Dispose();
        }

        public void Hotkeys(string code)
        {
            plugin?.Hotkeys(code);
        }

        public void Init()
        {
            HotKeyManagement hotKeyManagement = HotKeyManagement.GetInstance();
            foreach (var setting in Settings)
            {
                if (setting.Type == SettingType.Hotkey)
                {
                    hotKeyManagement.RegisterHotKey(setting.Value, this, setting.Key);
                }
            }
            plugin?.Init();
        }
    }
}
