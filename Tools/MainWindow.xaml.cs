using System.ComponentModel;
using System.Windows;

namespace Tools
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = MainWindowViewModel.GetInstance();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //TODO:测试时注释
            //this.Hide();
            HotKeyManagement.Handle = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            FileListenerServer.dispatcher = this.Dispatcher;
            PluginOperate.GetInstance("Plugin").LoadPlugin();
        }

        private void Setting_Show(object sender, RoutedEventArgs e)
        {
            this.Show();
            this.Activate();
        }

        bool cancel = true;

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            cancel = false;
            this.Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = cancel;
            if (cancel)
            {
                this.Hide();
                return;
            }
            PluginOperate.GetInstance().DisposePlugin();
        }
    }
}
