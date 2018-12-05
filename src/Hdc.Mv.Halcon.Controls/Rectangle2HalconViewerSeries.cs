using System.Windows;
using HalconDotNet;

namespace Hdc.Controls
{
    public class Rectangle2HalconViewerSeries : HalconViewerSeries
    {
        #region XPath
        /// <summary>
        /// Row
        /// </summary>
        public string XPath
        {
            get { return (string) GetValue(XPathProperty); }
            set { SetValue(XPathProperty, value); }
        }

        public static readonly DependencyProperty XPathProperty = DependencyProperty.Register(
            "XPath", typeof (string), typeof (Rectangle2HalconViewerSeries));

        #endregion

        #region YPath
        /// <summary>
        /// Column
        /// </summary>
        public string YPath
        {
            get { return (string) GetValue(YPathProperty); }
            set { SetValue(YPathProperty, value); }
        }

        public static readonly DependencyProperty YPathProperty = DependencyProperty.Register(
            "YPath", typeof (string), typeof (Rectangle2HalconViewerSeries));

        #endregion

        #region AnglePath
        /// <summary>
        /// Phi
        /// </summary>
        public string AnglePath
        {
            get { return (string) GetValue(AnglePathProperty); }
            set { SetValue(AnglePathProperty, value); }
        }

        public static readonly DependencyProperty AnglePathProperty = DependencyProperty.Register(
            "AnglePath", typeof (string), typeof (Rectangle2HalconViewerSeries));

        #endregion

        #region HalfWidthPath
        /// <summary>
        /// Length1
        /// </summary>
        public string HalfWidthPath
        {
            get { return (string) GetValue(HalfWidthPathProperty); }
            set { SetValue(HalfWidthPathProperty, value); }
        }

        public static readonly DependencyProperty HalfWidthPathProperty = DependencyProperty.Register(
            "HalfWidthPath", typeof (string), typeof (Rectangle2HalconViewerSeries));

        #endregion

        #region HalfHeightPath
        /// <summary>
        /// Length2
        /// </summary>
        public string HalfHeightPath
        {
            get { return (string) GetValue(HalfHeightPathProperty); }
            set { SetValue(HalfHeightPathProperty, value); }
        }

        public static readonly DependencyProperty HalfHeightPathProperty = DependencyProperty.Register(
            "HalfHeightPath", typeof (string), typeof (Rectangle2HalconViewerSeries));

        #endregion

        protected override HRegion GetDisplayRegion(object element)
        {
            throw new System.NotImplementedException();
        }
    }
}