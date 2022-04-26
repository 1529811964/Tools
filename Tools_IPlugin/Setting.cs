using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Tools_IPlugin
{
    public class Setting : ObservableRecipient
    {
        public string Key { get; }
        public SettingType Type { get; }
        public string _value;

        public Setting(string key, SettingType type, string value)
        {
            Key = key;
            Type = type;
            Value = value;
        }

        public string Value
        {
            get => _value;
            set { _value = value; OnPropertyChanged(); }
        }
    }

    public enum SettingType
    {
        String = 0,
        Switch = 1,
        Hotkey = 2
    }
}
