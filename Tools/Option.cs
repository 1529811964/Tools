using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tools_IPlugin;

namespace Tools
{
    /// <summary>
    /// Setting.xaml 的交互逻辑
    /// </summary>
    public partial class Option : UserControl
    {
        public static readonly DependencyProperty option_nameProperty =
      DependencyProperty.Register("option_name", typeof(string), typeof(Option), new PropertyMetadata(""));
        public static readonly DependencyProperty valueProperty =
            DependencyProperty.Register("value", typeof(string), typeof(Option), new PropertyMetadata(""));
        public static readonly DependencyProperty typeProperty =
            DependencyProperty.Register("type", typeof(SettingType), typeof(Option), new PropertyMetadata(SettingType.String));

        public string option_name { get; set; }

        public string value { get; set; }

        public SettingType type { get; set; }

        public Option()
        {
            InitializeComponent();
        }

        private ModifierKeys modifier = ModifierKeys.None;
        private Key LastKey = Key.None;
        HotKeyManagement keyManagement = HotKeyManagement.GetInstance();

        private void HotKey_GotKeyBoardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            string hotkey = (sender as TextBox).Text;
            keyManagement.UnregisterHotKey(hotkey, this.Content as ModPlugin, option_name);

            keyManagement.UnregisterAllHotKey();
        }

        private void HotKey_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            keyManagement.RegisterAllHotKey();

            TextBox HotKey = sender as TextBox;
            // 如果没有非功能按键，则设置为空
            if (HotKey.Text.EndsWith(" + ") || string.IsNullOrEmpty(HotKey.Text))
            {
                HotKey.Text = string.Empty;
            }
            else
            {
                // 注册快捷键
                keyManagement.RegisterHotKey(HotKey.Text, this.Content as ModPlugin, option_name);
            }
        }

        private void HotKey_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var keyvalue = e.Key == Key.System ? e.SystemKey : e.Key;
            // 如果是功能键则跳过
            switch (keyvalue)
            {
                case Key.Tab:
                case Key.LeftShift:
                case Key.RightShift:
                case Key.LeftCtrl:
                case Key.RightCtrl:
                case Key.LeftAlt:
                case Key.RightAlt:
                case Key.RWin:
                case Key.LWin:
                    return;
            }

            e.Handled = true;

            modifier = ModifierKeys.None;
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                modifier |= ModifierKeys.Control;
            }

            if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
            {
                modifier |= ModifierKeys.Alt;
            }

            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                modifier |= ModifierKeys.Shift;
            }

            LastKey = keyvalue;

            TextBox HotKey = sender as TextBox;

            if (modifier == ModifierKeys.None && keyvalue == Key.Back)
            {
                //如果单独按下了Key.Back则置空
                HotKey.Text = String.Empty;
            }
            else
            {
                bool isCtrl = (modifier & ModifierKeys.Control) == ModifierKeys.Control;
                bool isAlt = (modifier & ModifierKeys.Alt) == ModifierKeys.Alt;
                bool isShift = (modifier & ModifierKeys.Shift) == ModifierKeys.Shift;

                if (isCtrl && isAlt && isShift)
                {
                    HotKey.Text = "Ctrl + Alt + Shift + " + LastKey;
                }
                else if (isCtrl && isAlt)
                {
                    HotKey.Text = "Ctrl + Alt + " + LastKey;
                }
                else if (isCtrl && isShift)
                {
                    HotKey.Text = "Ctrl + Shift + " + LastKey;
                }
                else if (isAlt && isShift)
                {
                    HotKey.Text = "Alt + Shift + " + LastKey;
                }
                else if (isCtrl)
                {
                    HotKey.Text = "Ctrl + " + LastKey;
                }
                else if (isAlt)
                {
                    HotKey.Text = "Alt + " + LastKey;
                }
                else
                {
                    HotKey.Text = String.Empty;
                }
            }
        }
    }
}
