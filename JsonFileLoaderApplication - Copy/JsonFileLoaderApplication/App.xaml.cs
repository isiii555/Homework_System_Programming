using JsonFileLoaderApplication.ViewModels;
using JsonFileLoaderApplication.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace JsonFileLoaderApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            File.WriteAllText("text.txt", "salam");
            var view = new MainView();
            view.DataContext = new MainViewModel();
            view.ShowDialog();
        }
    }
}
