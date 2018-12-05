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
    public class CircleHalconViewerSeries : HalconViewerSeries
    {
        protected override HRegion GetDisplayRegion(object element)
        {
            var row = (double)GetPropertyValue(element, CenterYPath);
            var col = (double)GetPropertyValue(element, CenterXPath);
            var radius = (double)GetPropertyValue(element, RadiusPath);

            if (double.IsInfinity(row) || double.IsNaN(row))
            {
                row = 0;
            }

            if (double.IsInfinity(col) || double.IsNaN(col))
            {
                col = 0;
            }

            var circle = new HRegion();
            circle.GenCircle(row, col, radius);
            var circleBoundary = circle.Boundary("inner");
            circle.Dispose();
            return circleBoundary;
        }

        #region CenterYPath

        public string CenterYPath
        {
            get { return (string)GetValue(CenterYPathProperty); }
            set { SetValue(CenterYPathProperty, value); }
        }

        public static readonly DependencyProperty CenterYPathProperty = DependencyProperty.Register(
            "CenterYPath", typeof(string), typeof(LineHalconViewerSeries));

        #endregion

        #region CenterXPath

        public string CenterXPath
        {
            get { return (string)GetValue(CenterXPathProperty); }
            set { SetValue(CenterXPathProperty, value); }
        }

        public static readonly DependencyProperty CenterXPathProperty = DependencyProperty.Register(
            "CenterXPath", typeof(string), typeof(LineHalconViewerSeries));

        #endregion

        #region RadiusPath

        public string RadiusPath
        {
            get { return (string)GetValue(RadiusPathProperty); }
            set { SetValue(RadiusPathProperty, value); }
        }

        public static readonly DependencyProperty RadiusPathProperty = DependencyProperty.Register(
            "RadiusPath", typeof(string), typeof(LineHalconViewerSeries));

        #endregion
    }
}