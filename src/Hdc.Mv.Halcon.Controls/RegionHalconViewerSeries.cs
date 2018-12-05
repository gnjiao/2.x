using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using HalconDotNet;
using Hdc.Collections;
using Hdc.Mv;
using Hdc.Mv.Halcon;

namespace Hdc.Controls
{
    public class RegionHalconViewerSeries : HalconViewerSeries
    {
        protected override HRegion GetDisplayRegion(object element)
        {
            var region = (HRegion)GetPropertyValue(element, RegionPath);

            return region;
        }

        #region RegionPath

        public string RegionPath
        {
            get { return (string) GetValue(RegionPathProperty); }
            set { SetValue(RegionPathProperty, value); }
        }

        public static readonly DependencyProperty RegionPathProperty = DependencyProperty.Register(
            "RegionPath", typeof (string), typeof (RegionHalconViewerSeries));

        #endregion
    }
}