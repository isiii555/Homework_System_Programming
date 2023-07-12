using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FileTransferApplication.ViewModels
{
    public class MainViewModel : ViewModelBase

    {
        private long progressBarValue = 0;
        private long progressBarMaxValue = 100;
        private string sourceFilePath;
        private string destinationFilePath;

        public Thread TransferThread { get; set; }
        public long ProgressBarValue { get => progressBarValue; set => Set(ref progressBarValue, value); }
        public long ProgressBarMaxValue { get => progressBarMaxValue; set => Set(ref progressBarMaxValue, value); }
        public string SourceFilePath { get => sourceFilePath; set => Set(ref sourceFilePath, value); }
        public string DestinationFilePath { get => destinationFilePath; set => Set(ref destinationFilePath, value); }

        public MainViewModel()
        {
            TransferThread = new Thread(() =>
            {
                TransferFile();
            });
        }

        public RelayCommand OpenSourceFileCommand
        {
            get => new RelayCommand(() =>
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                SourceFilePath = openFileDialog.FileName;
            }
        });
        }

        public RelayCommand OpenDestinationFileCommand
        {
            get => new RelayCommand(() =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    DestinationFilePath = openFileDialog.FileName;
                }
            });
        }

        public RelayCommand SuspendCommand
        {
            get => new RelayCommand(() =>
            {
                try
                {
                    TransferThread?.Suspend();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "FileTransferApplication", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }

        public RelayCommand ResumeCommand
        {
            get => new RelayCommand(() =>
            {
                try
                {
                    TransferThread?.Resume();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "FileTransferApplication", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }

        public RelayCommand TransferCommand
        {
            get => new RelayCommand(() =>
            {
                if (TransferThread.ThreadState == ThreadState.Running)
                    MessageBox.Show("Please stop current transfer in order to create new one!", "FileTransferApplication", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                    TransferThread?.Start();
            });
        }

        public RelayCommand AbortCommand
        {
            get => new RelayCommand(() =>
            {
                if (TransferThread.ThreadState == ThreadState.Suspended)
                {
                    TransferThread?.Resume();
                    TransferThread?.Abort();
                }
                TransferThread?.Abort();
                TransferThread = new Thread(() =>
                {
                    TransferFile();
                });
                MessageBox.Show("Transfer is stopped", "FileTransferApplication", MessageBoxButton.OK, MessageBoxImage.Error);
                
                DestinationFilePath = null;
                SourceFilePath = null;
                ProgressBarMaxValue = 100;
                ProgressBarValue = default;
            });
        }

        public void TransferFile()
        {
            if (!File.Exists(SourceFilePath))
            {
                MessageBox.Show("Source file not found!", "FileTransferApplication", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (!File.Exists(DestinationFilePath))
            {
                MessageBox.Show("Destination file not found!", "FileTransferApplication", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (SourceFilePath == DestinationFilePath)
            {
                MessageBox.Show("Destination path and source path are same!", "FileTransferApplication", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var filestream = new FileStream(SourceFilePath, FileMode.Open, FileAccess.Read))
            {
                using (var filestream2 = new FileStream(DestinationFilePath, FileMode.Open, FileAccess.Write))
                {
                    filestream2.Seek(filestream2.Length, SeekOrigin.Current);
                    ProgressBarMaxValue = filestream.Length;
                    byte lengthArray = 10;
                    byte[] byteArray = null;
                    for (long i = 0; i < ProgressBarMaxValue; i += lengthArray)
                    {
                        byteArray = new byte[lengthArray];
                        filestream.Read(byteArray, 0, lengthArray);
                        filestream2.Write(byteArray, 0, lengthArray);
                        ProgressBarValue += lengthArray;
                        Thread.Sleep(5);

                    }
                    MessageBox.Show("File succesfully transfered","FileTransferApplication",MessageBoxButton.OK, MessageBoxImage.Information);
                    TransferThread = new Thread(() =>
                    {
                        TransferFile();
                    });
                    ProgressBarValue = default;
                    ProgressBarMaxValue = 100;
                    SourceFilePath = null;
                    DestinationFilePath = null;
                }    
            }

        }
    }

}
