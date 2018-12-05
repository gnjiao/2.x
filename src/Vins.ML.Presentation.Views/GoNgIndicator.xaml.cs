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

namespace Vins.ML.Presentation.Views
{
    /// <summary>
    /// Interaction logic for GoNgIndicator.xaml
    /// </summary>
    public partial class GoNgIndicator : UserControl
    {
        public GoNgIndicator()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IsNGProperty = DependencyProperty.Register(
            "IsNG", typeof (bool), typeof (GoNgIndicator), new PropertyMetadata(default(bool)));

        public bool IsNG
        {
            get { return (bool) GetValue(IsNGProperty); }
            set { SetValue(IsNGProperty, value); }
        }

        public static readonly DependencyProperty IsValidProperty = DependencyProperty.Register(
            "IsValid", typeof (bool), typeof (GoNgIndicator), new PropertyMetadata(default(bool)));

        public bool IsValid
        {
            get { return (bool) GetValue(IsValidProperty); }
            set { SetValue(IsValidProperty, value); }
        }
    }
}
