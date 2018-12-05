using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using HalconDotNet;
using Hdc.Collections;
using Hdc.Mv.Halcon;
using Enumerable = System.Linq.Enumerable;

namespace Hdc.Controls
{
    /// <summary>
    /// Interaction logic for HalconViewerSeries.xaml
    /// </summary>
    public abstract partial class HalconViewerSeries : Control
    {
        public static Dispatcher WorkDispatcher { get; set; }

        static HalconViewerSeries()
        {
            Thread newWindowThread = new Thread(new ThreadStart(() =>
            {
                Debug.WriteLine("ThreadStart begin");
                WorkDispatcher = Dispatcher.CurrentDispatcher;
                System.Windows.Threading.Dispatcher.Run();
                Debug.WriteLine("ThreadStart end");
            }));
            //            newWindowThread.Start();
        }

        public virtual void Refresh()
        {
            //            DisplayItems(ItemsSource);
        }

        public HalconViewer HalconViewer { protected get; set; }

        public static object GetPropertyValue(object src, string propName)
        {
            Type type = src.GetType();
            PropertyInfo propertyInfo = type.GetProperty(propName);
            return propertyInfo.GetValue(src, null);
        }

        #region ItemsSource

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource", typeof(IEnumerable), typeof(HalconViewerSeries),
            new PropertyMetadata(OnMeasurementItemsSourcePropertyChangedCallback));

        private static void OnMeasurementItemsSourcePropertyChangedCallback(DependencyObject s,
            DependencyPropertyChangedEventArgs e)
        {
            var me = s as HalconViewerSeries;
            if (me == null) return;

            //            me.DisplayItems(e.NewValue as IEnumerable);

            //
            var ds = e.NewValue as INotifyCollectionChanged;
            NotifyCollectionChangedEventHandler dsOnCollectionChanged =
                (x, y) => me.DisplayItems(ds as IEnumerable);

            if (ds != null)
            {
                ds.CollectionChanged += dsOnCollectionChanged;
            }

            var oldDs = e.OldValue as INotifyCollectionChanged;
            if (oldDs != null)
            {
                oldDs.CollectionChanged -= dsOnCollectionChanged;
            }
        }

        #endregion

        protected virtual void DisplayItems(IEnumerable enumerable)
        {
            Application.Current.MainWindow.Dispatcher.Invoke(() =>
            {
                if (enumerable == null)
                    return;

                var elements = enumerable as List<object> ?? enumerable.Cast<object>().ToList();

                if (!elements.Any())
                    return;

                BeforeDisplayItems(elements);

                List<HRegion> regions = new List<HRegion>();

                foreach (var element in elements)
                {
                    HRegion region;
                    var propertyValue = GetPropertyValue(element, "DisplayEnabled");
                    if (propertyValue != null)
                    {
                        var displayEnabled = (bool)propertyValue;
                        if (displayEnabled)
                        {
                            //                            DisplayItem(element);
                            region = GetDisplayRegion(element);
                            //                            HalconViewer.HWindowControlWpf.HalconWindow.DispObj(region);
                            if (region != null)
                                regions.Add(region);
                        }
                        else
                            continue;
                    }
                    else
                    {
                        //                        DisplayItem(element);
                        region = GetDisplayRegion(element);
                        //                        HalconViewer.HWindowControlWpf.HalconWindow.DispObj(region);
                        if (region != null)
                            regions.Add(region);
                    }

//                    if (region != null)
//                    {
//                        var xldRegion = region.GenContourRegionXld("border");
//                        xldRegion?.DispObj(HalconViewer.HWindowControlWpf.HalconWindow);
//                    }

                    HXLD xld = GetDisplayXLD(element);
                    xld?.DispObj(HalconViewer.HWindowControlWpf.HalconWindow);
                }

                var mergedRegion = regions.Union();
                mergedRegion.DispRegion(HalconViewer.HWindowControlWpf.HalconWindow);

//                var mergedRegion = regions.Union();
//                var mergedRegionXld = mergedRegion.GenContourRegionXld("border");
//                mergedRegionXld?.DispObj(HalconViewer.HWindowControlWpf.HalconWindow);

//                HalconViewer.HWindowControlWpf.HalconWindow.DispObj(mergedRegion);

                AfterDisplayItems(elements);
            });
        }

        protected virtual void AfterDisplayItems(List<object> elements)
        {
        }

        protected virtual void BeforeDisplayItems(List<object> elements)
        {
            HalconViewer.HWindowControl.HalconWindow.SetColor(ColorName);
            HalconViewer.HWindowControl.HalconWindow.SetDraw(RegionFillMode);
            HalconViewer.HWindowControl.HalconWindow.SetLineWidth(LineWidth);
        }

        protected abstract HRegion GetDisplayRegion(object element);

        protected virtual HXLD GetDisplayXLD (object element)
        {
            return null;
        }

        #region ColorName

        public string ColorName
        {
            get { return (string)GetValue(ColorNameProperty); }
            set { SetValue(ColorNameProperty, value); }
        }

        public static readonly DependencyProperty ColorNameProperty = DependencyProperty.Register(
            "ColorName", typeof(string), typeof(HalconViewerSeries), new FrameworkPropertyMetadata("cyan"));

        #endregion

        #region LineWidth

        public int LineWidth
        {
            get { return (int)GetValue(LineWidthProperty); }
            set { SetValue(LineWidthProperty, value); }
        }

        public static readonly DependencyProperty LineWidthProperty = DependencyProperty.Register(
            "LineWidth", typeof(int), typeof(HalconViewerSeries), new FrameworkPropertyMetadata(2));

        #endregion

        #region RegionFillMode

        public RegionFillMode RegionFillMode
        {
            get { return (RegionFillMode)GetValue(RegionFillModeProperty); }
            set { SetValue(RegionFillModeProperty, value); }
        }

        public static readonly DependencyProperty RegionFillModeProperty = DependencyProperty.Register(
            "RegionFillMode", typeof(RegionFillMode), typeof(HalconViewerSeries),
            new FrameworkPropertyMetadata(RegionFillMode.Margin));

        #endregion

        #region DisplayEnabled

        public bool DisplayEnabled
        {
            get { return (bool)GetValue(DisplayEnabledProperty); }
            set { SetValue(DisplayEnabledProperty, value); }
        }

        public static readonly DependencyProperty DisplayEnabledProperty = DependencyProperty.Register(
            "DisplayEnabled", typeof(bool), typeof(HalconViewerSeries));

        #endregion
    }
}