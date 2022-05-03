using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Tools
{
    public class ConfigureManagement
    {
        private struct Xmlnode
        {
            public string Name;
            public string AttributeName;
            public string AttributeValue;
            public Xmlnode(string name, string attribute)
            {
                Name = name;
                AttributeName = "Key";
                AttributeValue = attribute;
            }
            public Xmlnode(string name, string attributeName, string attributeValue)
            {
                Name = name;
                AttributeName = attributeName;
                AttributeValue = "true";
            }
        }

        private string Filename { get; } = "setting.xml";

        private static ConfigureManagement configureManagement = new ConfigureManagement();

        private Dictionary<Xmlnode, string> configs = new Dictionary<Xmlnode, string>();

        private ConfigureManagement() : this("setting.xml") { }

        private ConfigureManagement(string filename)
        {
            this.Filename = filename;
            var file = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), filename));
            if (!file.Exists)
            {
                file.Create();
                return;
            }
            // 遍历xml文件
            var xml = XDocument.Load(file.FullName);
            var root = xml.Root;
            var nodes = root.Elements();
            foreach (var node in nodes)
            {
                var key = node.Name.ToString();
                var attributename = node.FirstAttribute.Name.ToString();
                var attributevalue = node.FirstAttribute.Value;
                var value = node.Value;
                if (attributename == "Key")
                {
                    configs.Add(new Xmlnode(key, attributevalue), value);
                }
                else if (attributename == "Active")
                {
                    configs.Add(new Xmlnode(key, attributename, attributevalue), value);
                }
            }
        }

        public static ConfigureManagement GetInstance()
        {
            if (configureManagement == null)
            {
                configureManagement = new ConfigureManagement();
            }
            return configureManagement;
        }

        public static ConfigureManagement GetInstance(string filename)
        {
            if (configureManagement == null)
            {
                configureManagement = new ConfigureManagement(filename);
                return configureManagement;
            }
            if (configureManagement.Filename != filename)
            {
                configureManagement = new ConfigureManagement(filename);
            }
            return configureManagement;
        }
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="name">插件名</param>
        /// <param name="attribute">配置名</param>
        /// <returns>如不存在此配置则返回null</returns>
        public string GetConfig(string name, string attribute)
        {
            var node = new Xmlnode(name, attribute);
            if (configs.ContainsKey(node))
            {
                return configs[node];
            }
            return null;
        }
        public bool GetActive(string name)
        {
            var node = new Xmlnode(name, "Active", "true");
            if (configs.ContainsKey(node))
            {
                return configs[node].ToLower() == "true";
            }
            return false;
        }
        /// <summary>
        /// 清空配置
        /// </summary>
        public void ClearConfig()
        {
            configs.Clear();
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        public void SaveConfig()
        {
            ClearConfig();
            var file = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), Filename));
            if (!file.Exists)
            {
                file.Create();
            }
            var pluginconfig = MainWindowViewModel.GetInstance().Plugins;
            foreach (var plugin in pluginconfig)
            {
                configs.Add(new Xmlnode(plugin.Name, "Active", "true"), plugin.Active.ToString());
                foreach (var setting in plugin.Settings)
                {
                    configs.Add(new Xmlnode(plugin.Name, setting.Key), setting.Value);
                }
            }
            var xml = new XDocument();
            var root = new XElement("Setting");
            foreach (var config in configs)
            {
                var node = new XElement(config.Key.Name);
                node.SetAttributeValue(config.Key.AttributeName, config.Key.AttributeValue);
                node.Value = config.Value;
                root.Add(node);
            }
            xml.Add(root);
            xml.Save(file.FullName);
        }
    }
}
