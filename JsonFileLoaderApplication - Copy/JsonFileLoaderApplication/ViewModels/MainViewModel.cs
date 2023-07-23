using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace JsonFileLoaderApplication.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private TimeSpan time;
        private List<Car> cars;

        public List<Car> Cars { get => cars; set => Set(ref cars, value); }

        public bool Choice { get; set; }

        public TimeSpan Time { get => time; set => Set(ref time, value); }

        public MainViewModel()
        {
            Cars = new List<Car>();
        }

        public void LoadJson(int j)
        {
            Thread.CurrentThread.IsBackground = false;
            if (j < 6)
            {
                var str = File.ReadAllText($"cars{j}.json");
                var cars = JsonSerializer.Deserialize<List<Car>>(str);
                Cars.AddRange(cars);
                MessageBox.Show($"{j} , {Thread.CurrentThread.ManagedThreadId} , {Cars.Count}");
            }
        }

        public RelayCommand StartCommand
        {
            get => new RelayCommand(() =>
        {
            var stopwatch = new Stopwatch();
            if (Choice)
            {
                stopwatch.Start();
                //Thread thread = new Thread(() =>
                //{
                using (var countdown = new CountdownEvent(5))
                {
                    for (int i = 1; i <= 5; i++)
                    {
                        ThreadPool.QueueUserWorkItem((e) =>
                        {
                            LoadJson(i);
                            countdown.Signal();
                        });
                    }
                    countdown.Wait();
                }
                //});
                //thread.Start();
                //thread.Join();
                Cars = new List<Car>(Cars);
            }
            else
            {
                stopwatch.Start();
                var thread = new Thread(() =>
                {
                    for (int i = 1; i <= 5; i++)
                    {
                        var str = File.ReadAllText($"cars{i}.json");
                        var cars = JsonSerializer.Deserialize<List<Car>>(str);
                        Cars.AddRange(cars);
                    }
                    Cars = new List<Car>(Cars);
                });
                thread.Start();
                thread.Join();
                stopwatch.Stop();
            }
            Time = stopwatch.Elapsed;
        });
        }
    }
}
