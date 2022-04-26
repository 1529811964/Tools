using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Tools_IPlugin
{
    public abstract class IPlugin : ObservableRecipient, IDisposable
    {
        /// <summary>
        /// 插件名称
        /// </summary>
        public string Name { get; protected set; }
        /// <summary>
        /// 插件版本
        /// </summary>
        public string Version { get; protected set; }
        /// <summary>
        /// 插件作者
        /// </summary>
        public string Author { get; protected set; }
        /// <summary>
        /// 插件生成日期
        /// </summary>
        public DateTime LastTime { get; protected set;  }
        string _active;
        /// <summary>
        /// 插件启用状态
        /// </summary>
        public string Active { get => _active; set { _active = value; OnPropertyChanged(); } }
        /// <summary>
        /// 插件配置，应当在构造函数中初始化为默认配置
        /// </summary>
        public ObservableCollection<Setting> Settings { get; set; } = new ObservableCollection<Setting>();

        /// <summary>
        /// 启动插件
        /// </summary>
        /// <param name="info">启动插件时所需的配置信息</param>
        public abstract void Init();

        /// <summary>
        /// 禁用插件
        /// </summary>
        public abstract void Disabled();

        /// <summary>
        /// 执行快捷键功能
        /// </summary>
        /// <param name="code">快捷键代码</param>
        public abstract void Hotkeys(string code);
        
        public abstract void Dispose();
    }
}
