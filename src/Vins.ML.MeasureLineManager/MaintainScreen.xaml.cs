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
using Hdc;

namespace Vins.ML.MeasureLineManager
{
    /// <summary>
    /// Interaction logic for MaintainScreen.xaml
    /// </summary>
    public partial class MaintainScreen : UserControl
    {
        public MaintainScreen()
        {
            InitializeComponent();
        }

        private void SwitchToMonitorScreenButton_OnClick(object sender, RoutedEventArgs e)
        {
            MainViewModel.Singleton.SwitchScreenTo(0);
        }

        private void SendIndexOfPlateOfDownloadRobotButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (IndexOfOkPlateOfDownloadRobotTextBox.Text.IsNullOrEmpty())
            {
                MessageBox.Show("OK盘序号为空，请输入");
                return;
            }

            if (IndexOfNgPlateOfDownloadRobotTextBox.Text.IsNullOrEmpty())
            {
                MessageBox.Show("NG盘序号为空，请输入");
                return;
            }

            //
            bool ret;
            int indexOfOkPlateOfDownloadRobot = 0;
            ret = Int32.TryParse(IndexOfOkPlateOfDownloadRobotTextBox.Text, out indexOfOkPlateOfDownloadRobot);
            if (!ret)
            {
                MessageBox.Show("OK盘序号不是整数，请输入");
                return;
            }

            //
            int indexOfNgPlateOfDownloadRobot = 0;
            ret = Int32.TryParse(IndexOfNgPlateOfDownloadRobotTextBox.Text, out indexOfNgPlateOfDownloadRobot);
            if (!ret)
            {
                MessageBox.Show("NG盘序号不是整数，请输入");
                return;
            }

            MainViewModel.Singleton.SendIndexesOfPlateOfDownloadRobot(indexOfOkPlateOfDownloadRobot,
                indexOfNgPlateOfDownloadRobot);
        }
    }
}
