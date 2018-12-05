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
using Microsoft.Win32;
using Vins.ML.Domain;

namespace Vins.ML.MeasureLineManager
{
    /// <summary>
    /// Interaction logic for ReportScreen.xaml
    /// </summary>
    public partial class ReportScreen : UserControl
    {
        public ReportScreen()
        {
            InitializeComponent();
        }

        private void CreateSampleMeasureResultButton_OnClick(object sender, RoutedEventArgs e)
        {
            MainViewModel.Singleton.CreateSampleMeasureResult();
        }

        private void QueryAllWorkpieceResultsButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                MainViewModel.Singleton.QueryAllWorkpieceResults();
            }
            catch (Exception exception)
            {
                MessageBox.Show($"{nameof(MainViewModel.QueryAllWorkpieceResults)} is invalid." + 
                                $"\n" + exception.Message);
                return;
            }
        }

        private void GetWorkpieceResultButton_OnClick(object sender, RoutedEventArgs e)
        {
            int wpcTag;
            try
            {
                wpcTag = int.Parse(GetWorkpieceResultTextBox.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("WorkpieceTag is invalid.");
                return;
            }

            MainViewModel.Singleton.QueryWorkpieceResultByTag(wpcTag);
        }

        private void GetWorkpieceResultsByTakeButton_OnClick(object sender, RoutedEventArgs e)
        {
            int wpcTag;
            int count;
            try
            {
                wpcTag = int.Parse(GetWorkpieceResultsByTake_StartTag_TextBox.Text);
                count = int.Parse(GetWorkpieceResultsByTake_Count_TextBox.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("StartTag or Count is invalid.");
                return;
            }

            try
            {
                MainViewModel.Singleton.GetWorkpieceResultsByTake(wpcTag, count);
            }
            catch (Exception exception)
            {
                MessageBox.Show($"{nameof(MainViewModel.GetWorkpieceResultsByTake)} is invalid." +
                                $"\n" + exception.Message);
            }
        }

        private void SwitchToMainScreenFromReportScreenButton_OnClick(object sender, RoutedEventArgs e)
        {
            MainViewModel.Singleton.SwitchScreenTo(0);
        }

        private void GetWorkpieceResultsByBetweenButton_OnClick(object sender, RoutedEventArgs e)
        {
            int beginWpcTag;
            int endWpcTag;
            try
            {
                beginWpcTag = int.Parse(GetWorkpieceResultsByBetween_BeginTag_TextBox.Text);
                endWpcTag = int.Parse(GetWorkpieceResultsByBetween_EndTag_TextBox.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("beginWpcTag or endWpcTag is invalid.");
                return;
            }

            try
            {
                MainViewModel.Singleton.GetWorkpieceResultsByBetween(beginWpcTag, endWpcTag);
            }
            catch (Exception exception)
            {
                MessageBox.Show($"{nameof(MainViewModel.GetWorkpieceResultsByBetween)} is invalid." +
                                $"\n" + exception.Message);
            }
        }

        private void ExportWorkpieceResultsButton_OnClick(object sender, RoutedEventArgs e)
        {
            var text = ExportDirectoryTextBox.Text;
            Task.Run(() =>
            {
                var ret = MainViewModel.Singleton.ExportWorkpieceResults(text);
                if (ret)
                    MessageBox.Show("Export OK: " + text);
            });
        }

        private void ExportWorkpieceResultsByBetweenButton_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ExportWorkpieceResultsByTakeButton_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void RemoveWorkpieceResultsButton_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ExportWorkpieceResultButton_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void GetWorkpieceResultsByTakeLast_Query_OnClick(object sender, RoutedEventArgs e)
        {
            int count;
            try
            {
                count = int.Parse(GetWorkpieceResultsByTakeLast_Count_TextBox.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Count is invalid.");
                return;
            }

            try
            {
                MainViewModel.Singleton.GetWorkpieceResultsByTakeLast(count);
            }
            catch (Exception exception)
            {
                MessageBox.Show($"{nameof(MainViewModel.GetWorkpieceResultsByTakeLast)} is invalid." +
                                $"\n" + exception.Message);
            }
        }

        private void GetWorkpieceResultsByTakeLast_Export_Button_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ExportWorkpieceResultsCsvButton_OnClick(object sender, RoutedEventArgs e)
        {
            var text = ExportDirectoryTextBox.Text;
            Task.Run(() =>
            {
                var ret = MainViewModel.Singleton.ExportWorkpieceResultsToCsv(text);
                if (ret)
                    MessageBox.Show("Export Csv OK: " + text);
            });
        }
    }
}