using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tools_IPlugin;

namespace Tools
{
    /// <summary>
    /// Setting.xaml 的交互逻辑
    /// </summary>
    public partial class Option : UserControl
    {
        public static readonly DependencyProperty keyProperty =
      DependencyProperty.Register("key", typeof(string), typeof(Option), new PropertyMetadata(""));
        public static readonly DependencyProperty valueProperty =
            DependencyProperty.Register("value", typeof(string), typeof(Option), new PropertyMetadata(""));
        public static readonly DependencyProperty typeProperty =
            DependencyProperty.Register("type", typeof(SettingType), typeof(Option), new PropertyMetadata(SettingType.String));
        public static readonly DependencyProperty codeProperty =
            DependencyProperty.Register("code", typeof(string), typeof(Option), new PropertyMetadata(""));

        public string key
        {
            get;
            set;
        }

        public string value
        {
            get;
            set;
        }

        public SettingType type
        {
            get;
            set;
        }

        public string code
        {
            get;
            set;
        }

        public Option()
        {
            InitializeComponent();
        }
    }
}
