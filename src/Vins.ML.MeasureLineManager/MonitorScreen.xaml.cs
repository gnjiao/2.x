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

namespace Vins.ML.MeasureLineManager
{
    /// <summary>
    /// Interaction logic for MonitorScreen.xaml
    /// </summary>
    public partial class MonitorScreen : UserControl
    {
        public MonitorScreen()
        {
            InitializeComponent();

            //MainViewModel.Singleton.QueryInitWorkCountResults();

            WorkcountViewGrid.DataContext = MainViewModel.Singleton.PlcWorkDataCount;

            WorkcountViewGrid.Visibility = MainViewModel.Singleton.Config.ShowWorkCountView;
        }


        private void SwitchToReportScreenFromMainScreenButton_OnClick(object sender, RoutedEventArgs e)
        {
            MainViewModel.Singleton.SwitchScreenTo(2);
        }

        private void SwitchToMaintainScreenFromMainScreenButton_OnClick(object sender, RoutedEventArgs e)
        {
            MainViewModel.Singleton.SwitchScreenTo(3);
        }

        private void ResetTotalCountButton_OnClick(object sender, RoutedEventArgs e)
        {
            MainViewModel.Singleton.ResetTotalCount();
        }

        private void ResetJobCountButton_OnClick(object sender, RoutedEventArgs e)
        {
            MainViewModel.Singleton.ResetJobCount();
        }
    }
}
