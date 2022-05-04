using System;
using System.IO;
using System.Windows.Threading;

namespace Tools
{
    public class FileListenerServer
    {
        ///<summary>
        ///文件监听
        /// </summary>
        private FileSystemWatcher _watcher;

        private MainWindowViewModel model;

        private PluginOperate pluginOperate;

        public static Dispatcher dispatcher;

        public FileListenerServer(string path)
        {
            try
            {
                this._watcher = new FileSystemWatcher(path);
                _watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.DirectoryName;
                // 不监视目标目录的子目录
                //_watcher.IncludeSubdirectories = true;
                _watcher.Created += new FileSystemEventHandler(FileWatcher_Created);
                _watcher.Changed += new FileSystemEventHandler(FileWatcher_Changed);
                _watcher.Deleted += new FileSystemEventHandler(FileWatcher_Deleted);
                _watcher.Renamed += new RenamedEventHandler(FileWatcher_Renamed);
            }
            catch (Exception ex)
            {
                HandyControl.Controls.MessageBox.Error(ex.Message, "文件夹监控失败");
            }
        }

        public void Start()
        {
            model = MainWindowViewModel.GetInstance();
            pluginOperate = PluginOperate.GetInstance();
            //开始监听
            this._watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            if (this._watcher == null)
            {
                return;
            }
            //关闭监听
            this._watcher.EnableRaisingEvents = false;
            this._watcher.Dispose();
            this._watcher = null;
        }

        /// <summary>
        /// 添加插件事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            var dll = new FileInfo(e.FullPath);
            if (dll.Extension != ".dll")
            {
                return;
            }
            dispatcher?.Invoke(() => pluginOperate.LoadPlugin(dll.FullName));
        }
        /// <summary>
        /// 修改插件事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            var dll = new FileInfo(e.FullPath);
            if (dll.Extension != ".dll")
            {
                return;
            }

            dispatcher?.Invoke(() =>
            {
                // 卸载旧插件
                pluginOperate.DisposePlugin(dll.Name);
                // 加载新插件
                pluginOperate.LoadPlugin(e.FullPath);
            });

            //避免多次触发
            this._watcher.EnableRaisingEvents = false;
            this._watcher.EnableRaisingEvents = true;
        }
        /// <summary>
        /// 删除插件事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FileWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            var dll = new FileInfo(e.FullPath);
            if (dll.Extension != ".dll")
            {
                return;
            }
            dispatcher?.Invoke(() => pluginOperate.DisposePlugin(dll.Name));
        }
        /// <summary>
        /// 重命名文件事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FileWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            if (e.OldName.EndsWith(".dll") && e.Name.EndsWith(".dll"))
            {
                foreach (var plugin in model.Plugins)
                {
                    if (plugin.Filename == e.OldName)
                    {
                        plugin.Filename = e.Name;
                        break;
                    }
                }
                return;
            }
            dispatcher?.Invoke(() =>
            {
                if (e.OldName.EndsWith(".dll") && !e.Name.EndsWith(".dll"))
                {
                    pluginOperate.DisposePlugin(e.OldName);
                    return;
                }
                if (!e.OldName.EndsWith(".dll") && e.Name.EndsWith(".dll"))
                {
                    pluginOperate.LoadPlugin(e.FullPath);
                    return;
                }
            });
        }
    }
}
