using System;
using System.IO;
using System.Reflection;
using Tools_IPlugin;

namespace Tools
{
    public class PluginOperate
    {
        public string Plugin_Dir { get; }

        private MainWindowViewModel model = MainWindowViewModel.GetInstance();

        private static PluginOperate pluginoperate = new PluginOperate("Plugin");

        private FileListenerServer listenerServer;

        private PluginOperate(string plugin_dir)
        {
            this.Plugin_Dir = plugin_dir;
            listenerServer = new FileListenerServer(Path.Combine(Directory.GetCurrentDirectory(), plugin_dir));
        }

        public static PluginOperate GetInstance()
        {
            if (pluginoperate == null)
            {
                pluginoperate = new PluginOperate("Plugin");
            }
            return pluginoperate;
        }

        public static PluginOperate GetInstance(string plugin_dir)
        {
            if (pluginoperate == null)
            {
                pluginoperate = new PluginOperate(plugin_dir);
                return pluginoperate;
            }
            if (pluginoperate.Plugin_Dir != plugin_dir)
            {
                pluginoperate = new PluginOperate(plugin_dir);
            }
            return pluginoperate;
        }

        /// <summary>
        /// 加载指定相对目录下的所有插件
        /// </summary>
        /// <returns>返回是否所有插件均加载成功</returns>
        public bool LoadPlugin()
        {
            bool result = true;
            var dir = Directory.GetCurrentDirectory();
            var path = Path.Combine(dir, this.Plugin_Dir);

            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            var dlls = directoryInfo.GetFiles("*.dll");
            foreach (var dll in dlls)
            {
                if (!LoadPlugin(dll.FullName))
                {
                    result = false;
                }
            }

            // 启动文件夹监听，实现插件的热插拔
            listenerServer.Start();

            return result;
        }
        /// <summary>
        /// 加载指定文件路径的插件
        /// </summary>
        /// <param name="plugin_path">插件文件完整目录</param>
        /// <returns>返回插件是否加载成功</returns>
        public bool LoadPlugin(string plugin_path)
        {
            ModPlugin plugin = null;
            bool result = false;
            // 如果文件类型不是dll文件则直接返回
            if (!plugin_path.EndsWith(".dll"))
            {
                return false;
            }

            try
            {
                // 加载插件
                var assembly = Assembly.Load(plugin_path);

                foreach (var item in assembly.GetTypes())
                {
                    if (!typeof(IPlugin).IsAssignableFrom(item))
                    {
                        continue;
                    }
                    plugin = new ModPlugin(Activator.CreateInstance(item) as IPlugin, new FileInfo(plugin_path).Name);
                    result = true;
                    break;
                }
                if (result)
                {
                    // 默认插件状态为禁用
                    plugin.Active = false;
                    var config = ConfigureManagement.GetInstance();
                    plugin.Active = config.GetActive(plugin.Name);
                    foreach (var setting in plugin.Settings)
                    {
                        if (config.GetConfig(plugin.Name, setting.Key) != null)
                        {
                            setting.Value = config.GetConfig(plugin.Name, setting.Key);
                        }
                    }
                    if (plugin.Active)
                    {
                        plugin.Init();
                    }
                    model.Plugins.Add(plugin);
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public void DisposePlugin()
        {
            //停止文件夹监听
            listenerServer.Stop();

            ConfigureManagement.GetInstance().SaveConfig();

            foreach (var plugin in model.Plugins)
            {
                plugin.Dispose();
            }

            HotKeyManagement.GetInstance().UnregisterAllHotKey();
        }

        /// <summary>
        /// 销毁指定文件名的插件
        /// </summary>
        /// <param name="plugin_name">文件名，不包含路径</param>
        public void DisposePlugin(string plugin_name)
        {
            foreach (var plugin in model.Plugins)
            {
                if (plugin.Filename == plugin_name)
                {
                    plugin.Dispose();
                    model.Plugins.Remove(plugin);
                    break;
                }
            }
        }
    }
}
