using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

namespace TaskManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string ProcessName { get; set;}

        public Process SelectedProcess { get; set; }

        public ObservableCollection<Process> Processes
        {
            get { return (ObservableCollection<Process>)GetValue(ProcessesProperty); }
            set { SetValue(ProcessesProperty, value); }
        }

        public static readonly DependencyProperty ProcessesProperty =
            DependencyProperty.Register("Processes", typeof(ObservableCollection<Process>), typeof(MainWindow));

        //public ObservableCollection<Process> Processes { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Processes = new ObservableCollection<Process>(Process.GetProcesses());
        }

        private void Create_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var process = Process.Start($"{ProcessName}");
                Processes.Add(process);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please provide valid process name");
            }
        }

        private void End_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SelectedProcess.Kill();
                Processes.Remove(SelectedProcess);
            }
            catch(Exception ex)
            {
                MessageBox.Show("The process could not be terminated.");
            }
        }
    }
}
