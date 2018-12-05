using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using HalconDotNet;
using Hdc.Mv.Halcon;

namespace Hdc.Controls
{
    public class LineHalconViewerSeries : HalconViewerSeries
    {
        protected override HRegion GetDisplayRegion(object element)
        {
            var row1 = (double)GetPropertyValue(element, StartPointYPath);
            var column1 = (double)GetPropertyValue(element, StartPointXPath);
            var row2 = (double)GetPropertyValue(element, EndPointYPath);
            var column2 = (double)GetPropertyValue(element, EndPointXPath);

            var line = new HRegion();
            line.GenRegionLine(row1, column1, row2, column2);

            return line;
        }

        #region StartPointYPath

        public string StartPointYPath
        {
            get { return (string) GetValue(StartPointYPathProperty); }
            set { SetValue(StartPointYPathProperty, value); }
        }

        public static readonly DependencyProperty StartPointYPathProperty = DependencyProperty.Register(
            "StartPointYPath", typeof (string), typeof (LineHalconViewerSeries));

        #endregion

        #region StartPointXPath

        public string StartPointXPath
        {
            get { return (string) GetValue(StartPointXPathProperty); }
            set { SetValue(StartPointXPathProperty, value); }
        }

        public static readonly DependencyProperty StartPointXPathProperty = DependencyProperty.Register(
            "StartPointXPath", typeof (string), typeof (LineHalconViewerSeries));

        #endregion

        #region EndPointYPath

        public string EndPointYPath
        {
            get { return (string) GetValue(EndPointYPathProperty); }
            set { SetValue(EndPointYPathProperty, value); }
        }

        public static readonly DependencyProperty EndPointYPathProperty = DependencyProperty.Register(
            "EndPointYPath", typeof (string), typeof (LineHalconViewerSeries));

        #endregion

        #region EndPointXPath

        public string EndPointXPath
        {
            get { return (string) GetValue(EndPointXPathProperty); }
            set { SetValue(EndPointXPathProperty, value); }
        }

        public static readonly DependencyProperty EndPointXPathProperty = DependencyProperty.Register(
            "EndPointXPath", typeof (string), typeof (LineHalconViewerSeries));

        #endregion
    }
}