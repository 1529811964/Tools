using System;
using System.Collections.ObjectModel;
using Tools_IPlugin;

namespace Tools
{
    public class MainWindowViewModel
    {
        public static MainWindowViewModel ViewModel { get; private set; } = new MainWindowViewModel();

        public ObservableCollection<ModPlugin> Plugins { get; set; } = new ObservableCollection<ModPlugin>();

        public static MainWindowViewModel GetInstance()
        {
            if (ViewModel == null)
            {
                ViewModel = new MainWindowViewModel();
            }
            return ViewModel;
        }

        private MainWindowViewModel()
        {
        }
    }
}
