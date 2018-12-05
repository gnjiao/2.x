using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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
using EasyNetQ;
using Hdc.Collections.Generic;
using Hdc.ComponentModel;
using Hdc.Measuring;
using Hdc.Measuring.VinsML;
using Hdc.Mercury;
using Hdc.Mvvm.Resources;
using Hdc.Patterns;
using Hdc.Reactive.Linq;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Vins.ML.Domain;
using Vins.ML.MeasureLineManager.Annotations;

namespace Vins.ML.MeasureLineManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        public MainWindow()
        {
            DrawingBrushServiceLocator.Loader = new XamlDrawingBrushLoader();
            
            InitializeComponent();

            DataContext = MainViewModel.Singleton;

            this.Closed += MainWindow_Closed;
        }


        private void MainWindow_Closed(object sender, EventArgs e)
        {
            try
            {
                MainViewModel.Singleton.SaveTotalCount();
            }
            finally
            {
                Environment.Exit(0);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        private void StartMqButton_OnClick(object sender, RoutedEventArgs e)
        {
            MainViewModel.Singleton.StartMqButton();
            StartMqButton.Content = "已连接";
            StartMqButton.IsEnabled = false;
        }

    }
}