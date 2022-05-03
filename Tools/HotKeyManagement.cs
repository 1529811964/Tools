using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Interop;

namespace Tools
{
    public class HotKeyManagement
    {
        private struct Hotkeymod
        {
            public Hotkeymod(ModPlugin modPlugin, string key)
            {
                this.Modplugin = modPlugin;
                this.key = key;
            }
            public ModPlugin Modplugin;
            public string key;
        }

        private static HotKeyManagement keyManagement = new HotKeyManagement();

        private Dictionary<int, List<Hotkeymod>> HotKeyList = new Dictionary<int, List<Hotkeymod>>();

        public static IntPtr Handle;

        private HotKeyManagement()
        {
            ComponentDispatcher.ThreadPreprocessMessage += Execute_hotkey;
        }

        public static HotKeyManagement GetInstance()
        {
            if (keyManagement == null)
            {
                keyManagement = new HotKeyManagement();
            }
            return keyManagement;
        }
        /// <summary>
        /// 快捷键功能执行函数
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="Handled"></param>
        private static void Execute_hotkey(ref MSG Message, ref bool Handled)
        {
            if (Message.message == 786)
            {
                var id = Message.wParam.ToInt32();
                if (keyManagement.HotKeyList.ContainsKey(id))
                {
                    var HotKeyList = keyManagement.HotKeyList[id];
                    foreach (Hotkeymod hotkeymod in HotKeyList)
                    {
                        hotkeymod.Modplugin.Hotkeys(hotkeymod.key);
                    }
                }
            }
        }
        /// <summary>
        /// 启用所有快捷键
        /// </summary>
        /// <returns>返回是否全部快捷键注册成功</returns>
        public bool RegisterAllHotKey()
        {
            bool result = true;
            foreach (int id in HotKeyList.Keys)
            {
                KeyModifiers fsModifiers = (KeyModifiers)(id & 0xF);
                fsModifiers |= KeyModifiers.Norepeat;
                Keys vk = (Keys)(id / 0x10);
                if (!RegisterHotKey(Handle, id, fsModifiers, vk))
                {
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// 注册快捷键
        /// </summary>
        /// <param name="hotkey">快捷键</param>
        /// <param name="modPlugin">使用快捷键的插件</param>
        /// <param name="key">使用快捷键的功能名称</param>
        /// <returns>注册成功则返回true,反之为false</returns>
        public bool RegisterHotKey(string hotkey, ModPlugin modPlugin, string key)
        {
            int id = GetHotkeyid(hotkey);
            if (id == -1)
            {
                return false;
            }
            if (HotKeyList.ContainsKey(id))
            {
                HotKeyList[id].Add(new Hotkeymod(modPlugin, key));
                return true;
            }
            else
            {
                KeyModifiers fsModifiers = (KeyModifiers)(id & 0xF);
                fsModifiers |= KeyModifiers.Norepeat;
                Keys vk = (Keys)(id / 0x10);
                if (!RegisterHotKey(Handle, id, fsModifiers, vk))
                {
                    return false;
                }
                else
                {
                    HotKeyList[id] = new List<Hotkeymod>() { new Hotkeymod(modPlugin, key) };
                    return true;
                }
            }
        }
        /// <summary>
        /// 禁用所有快捷键
        /// </summary>
        /// <returns>返回是否全部快捷键禁用成功</returns>
        public bool UnregisterAllHotKey()
        {
            bool result = true;
            foreach (int id in HotKeyList.Keys)
            {
                if (!UnregisterHotKey(Handle, id))
                {
                    result = false;
                }
            }
            return result;
        }
        /// <summary>
        /// 注销快捷键
        /// </summary>
        /// <param name="hotkey">快捷键</param>
        /// <param name="modPlugin">使用快捷键的插件</param>
        /// <param name="key">使用快捷键的功能名称</param>
        /// <returns>注销成功则返回true,反之为false</returns>
        public bool UnregisterHotKey(string hotkey, ModPlugin modPlugin, string key)
        {
            int id = GetHotkeyid(hotkey);
            if (!HotKeyList.ContainsKey(id))
            {
                return true;
            }
            else
            {
                if (HotKeyList[id].Count == 1)
                {
                    if (!UnregisterHotKey(Handle, id))
                    {
                        return false;
                    }
                    else
                    {
                        HotKeyList.Remove(id);
                        return true;
                    }
                }
                HotKeyList[id].Remove(new Hotkeymod(modPlugin, key));
                return true;
            }
        }

        /// <summary>
        /// 根据给出的快捷键字符串返回相应的id
        /// </summary>
        /// <param name="hotkey">快捷键</param>
        /// <returns>如快捷键字符串不符合预期则返回-1</returns>
        int GetHotkeyid(string hotkey)
        {
            if (string.IsNullOrEmpty(hotkey))
            {
                return -1;
            }
            var key = hotkey.Split(new char[] { '+', ' ' });
            if (key.Length <= 1 || key.Length > 4)
            {
                return -1;
            }

            KeyModifiers fsModifiers = KeyModifiers.None;
            for (int i = 0; i < key.Length - 1; i++)
            {
                switch (key[i])
                {
                    case "Ctrl":
                        fsModifiers |= KeyModifiers.Ctrl;
                        break;
                    case "Alt":
                        fsModifiers |= KeyModifiers.Alt;
                        break;
                    case "Shift":
                        fsModifiers |= KeyModifiers.Shift;
                        break;
                    default:
                        break;
                }
            }
            int id = (int)fsModifiers;
            Keys vk = (Keys)Enum.Parse(typeof(Keys), key[key.Length - 1], true);
            id |= ((int)vk * 0x10);
            return id;
        }


        //如果函数执行成功，返回值不为0。
        //如果函数执行失败，返回值为0。要得到扩展错误信息，调用GetLastError。
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(
            IntPtr hWnd,                //要定义热键的窗口的句柄
            int id,                     //定义热键ID（不能与其它ID重复）
            KeyModifiers fsModifiers,   //标识热键是否在按Alt、Ctrl、Shift、Windows等键时才会生效
            Keys vk                     //定义热键的内容
            );
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(
            IntPtr hWnd,                //要取消热键的窗口的句柄
            int id                      //要取消热键的ID
            );
        //定义了辅助键的名称（将数字转变为字符以便于记忆，也可去除此枚举而直接使用数值）
        [Flags()]
        public enum KeyModifiers
        {
            None = 0x0000,
            Alt = 0x0001,
            Ctrl = 0x0002,
            Norepeat = 0x4000,
            Shift = 0x0004,
            WindowsKey = 0x0008
        }
    }
}
