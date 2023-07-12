using FileTransferApplication.ViewModels;
using FileTransferApplication.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FileTransferApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var view = new MainView();
            view.DataContext = new MainViewModel();
            view.ShowDialog();
            base.Shutdown();
        }
    }

}
