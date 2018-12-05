using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HalconDotNet;

namespace Hdc.Controls
{
    /// <summary>
    /// Interaction logic for HalconViewer.xaml
    /// </summary>
    [ContentProperty("Series")]
    public partial class HalconViewer : UserControl
    {
        private readonly ObservableCollection<HalconViewerSeries> _series =
            new ObservableCollection<HalconViewerSeries>();

        private bool dragDoing;
        private double mouseX;
        private double mouseY;
        private double mouseRow;
        private double mouseColumn;

        public HalconViewer()
        {
            InitializeComponent();

            HWindowControlWpf.MouseLeave += HalconViewer_MouseLeave;
            _series.CollectionChanged += _series_CollectionChanged;
        }

        private void _series_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var newItem in e.NewItems)
            {
                var indicatorSeriesBase = newItem as HalconViewerSeries;
                if (indicatorSeriesBase == null) continue;

                indicatorSeriesBase.HalconViewer = this;
                SeriesCanvas.Children.Add(indicatorSeriesBase);
            }

            Refresh();
        }

        private void HWindowControlWPF_OnHInitWindow(object sender, EventArgs e)
        {
            HWindowControlWpf.HMouseDown += HWindowControlWpf_HMouseDown;
            HWindowControlWpf.HMouseMove += HWindowControlWpf_HMouseMove;
            HWindowControlWpf.HMouseUp += HWindowControlWpf_HMouseUp;
            HWindowControlWpf.HMouseWheel += HWindowControlWpf_HMouseWheel;
        }

        #region Image

        public HImage Image
        {
            get { return (HImage)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(
            "Image", typeof(HImage), typeof(HalconViewer), new FrameworkPropertyMetadata(OnImageChangedCallback));

        private static void OnImageChangedCallback(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            var me = s as HalconViewer;
            if (me == null) return;

                me.HWindowControlWpf.HalconWindow.ClearWindow();

                var image = e.NewValue as HImage;
                if (image == null || image.Key == IntPtr.Zero)
                {
                    me.HWindowControlWpf.HalconWindow.DetachBackgroundFromWindow();
                }
                else
                {
                    me.HWindowControlWpf.HalconWindow.AttachBackgroundToWindow(image);
                }
                me.Refresh();
        }

        #endregion

        #region Scale

        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(
            "Scale", typeof(double), typeof(HalconViewer), new FrameworkPropertyMetadata(1.0, PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            var me = s as HalconViewer;

            if (me != null)
                me.Refresh();
        }

        #endregion

        #region X

        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        public static readonly DependencyProperty XProperty = DependencyProperty.Register(
            "X", typeof(double), typeof(HalconViewer), new FrameworkPropertyMetadata(PropertyChangedCallback));

        #endregion

        #region Y

        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        public static readonly DependencyProperty YProperty = DependencyProperty.Register(
            "Y", typeof(double), typeof(HalconViewer), new FrameworkPropertyMetadata(PropertyChangedCallback));

        #endregion

        public Collection<HalconViewerSeries> Series
        {
            get { return _series; }
            //            set { _series = value; }
        }

        public void ZoomFit()
        {
            dragDoing = false;

            if (Image == null)
                return;

            if (Image.Key == IntPtr.Zero)
                return;

            X = 0;
            Y = 0;

            int width, height;
            Image.GetImageSize(out width, out height);

            int rowExtent, columnExtent, widthExtent, heightExtent;
            HWindowControlWpf.HalconWindow.GetWindowExtents(out rowExtent, out columnExtent,
                out widthExtent, out heightExtent);

            var ratioImage = (double)width / (double)height;
            var ratioWindow = (double)widthExtent / (double)heightExtent;

            double finalRatio = 0;
            if (ratioImage > ratioWindow)
            {
                finalRatio = (double)widthExtent / (double)width;
                Y = 0 + (height - heightExtent / finalRatio) / 2;
            }
            else
            {
                finalRatio = (double)heightExtent / (double)height;
                X = 0 + (width - widthExtent / finalRatio) / 2;
            }

            Scale = finalRatio;

            Refresh();
        }

        public void ZoomFitFill()
        {
            dragDoing = false;

            if (Image == null)
                return;

            HWindowControlWpf.SetFullImagePart(Image);
        }

        public void ZoomIn()
        {
            dragDoing = false;

            int rowExtent, columnExtent, widthExtent, heightExtent;
            HWindowControlWpf.HalconWindow.GetWindowExtents(out rowExtent, out columnExtent,
                out widthExtent, out heightExtent);

            Thickness imagePart2 = HWindowControlWpf.ImagePart;

            var width1 = widthExtent / Scale;
            var height1 = heightExtent / Scale;

            Scale /= 0.75;

            var width2 = widthExtent / Scale;
            var height2 = heightExtent / Scale;


            var newMarginX = (width1 - width2) / 2.0;
            var newLeft = imagePart2.Left + newMarginX;

            var newMarginY = (height1 - height2) / 2.0;
            var newTop = imagePart2.Top + newMarginY;

            X = newLeft;
            Y = newTop;

            Refresh();
        }

        public void ZoomOut()
        {
            dragDoing = false;

            int rowExtent, columnExtent, widthExtent, heightExtent;
            HWindowControlWpf.HalconWindow.GetWindowExtents(out rowExtent, out columnExtent,
                out widthExtent, out heightExtent);

            Thickness imagePart2 = HWindowControlWpf.ImagePart;

            var width1 = widthExtent / Scale;
            var height1 = heightExtent / Scale;

            Scale *= 0.75;

            var width2 = widthExtent / Scale;
            var height2 = heightExtent / Scale;


            var newMarginX = (width2 - width1) / 2.0;
            var newLeft = imagePart2.Left - newMarginX;

            var newMarginY = (height2 - height1) / 2.0;
            var newTop = imagePart2.Top - newMarginY;

            X = newLeft;
            Y = newTop;

            Refresh();
        }

        public void ZoomActual()
        {
            dragDoing = false;

            X = 0;
            Y = 0;
            Scale = 1.0;

            Refresh();
        }

        private void Refresh()
        {
            if (Image == null)
                return;

            if (Image.Key == IntPtr.Zero)
                return;

            int width, height;
            Image.GetImageSize(out width, out height);

            int rowExtent, columnExtent, widthExtent, heightExtent;
            HWindowControlWpf.HalconWindow.GetWindowExtents(out rowExtent, out columnExtent,
                out widthExtent, out heightExtent);

            var imagePart2 = new Thickness
            {
                Left = X,
                Top = Y,
                Right = widthExtent / Scale,
                Bottom = heightExtent / Scale
            };

            HWindowControlWpf.ImagePart = imagePart2;

            foreach (var s in _series)
            {
                s.Refresh();
            }
        }

        private void HWindowControlWpf_HMouseWheel(object sender, HMouseEventArgsWPF e)
        {
            if (Image == null)
                return;

            //            int width, height;
            //            Image.GetImageSize(out width, out height);

            int rowExtent, columnExtent, widthExtent, heightExtent;
            HWindowControlWpf.HalconWindow.GetWindowExtents(out rowExtent, out columnExtent,
                out widthExtent, out heightExtent);

            var newX = e.X;
            var newY = e.Y;
            var newRow = e.Row;
            var newColumn = e.Column;

            Thickness imagePart2 = HWindowControlWpf.ImagePart;
            var xRatio = (newColumn - imagePart2.Left) / imagePart2.Right;
            var yRatio = (newRow - imagePart2.Top) / imagePart2.Bottom;

            if (e.Delta > 0)
            {
                Scale /= 0.75;
            }
            else if (e.Delta < 0)
            {
                Scale *= 0.75;
            }

            if (Scale > 100 || Scale < 0.01)
                return;


            var newMarginX = widthExtent / Scale * xRatio;
            var newLeft = newColumn - newMarginX;

            var newMarginY = heightExtent / Scale * yRatio;
            var newTop = newRow - newMarginY;

            X = newLeft;
            Y = newTop;

            //            imagePart2.Right = width * Scale;
            //            imagePart2.Bottom = height * Scale;
            //
            //            HWindowControlWpf.ImagePart = imagePart2;

            Refresh();
        }

        private void HWindowControlWpf_HMouseUp(object sender, HMouseEventArgsWPF e)
        {
            dragDoing = false;
        }

        private void HWindowControlWpf_HMouseMove(object sender, HMouseEventArgsWPF e)
        {
            //            if (CheckMouseIsOut(e))
            //                dragDoing = false;

            if (!dragDoing) return;

            var newX = e.X;
            var newY = e.Y;
            var newRow = e.Row;
            var newColumn = e.Column;

            var text3 = " newX   = " + newX.ToString("0000.0") +
                        " newY   = " + newY.ToString("0000.0") +
                        " newRow = " + newRow.ToString("0000.0") +
                        " newColumn = " + newColumn.ToString("0000.0");
            //            Debug.WriteLine("------------------------------------");
            //            Debug.WriteLine(text3);

            var moveLengthX = newX - mouseX;
            var moveLengthY = newY - mouseY;
            var moveLengthRow = newRow - mouseRow;
            var moveLengthColumn = newColumn - mouseColumn;

            X -= moveLengthColumn;
            Y -= moveLengthRow;

            Refresh();
        }

        private void HWindowControlWpf_HMouseDown(object sender, HMouseEventArgsWPF e)
        {
            if (dragDoing)
            {
                dragDoing = false;
                RaiseHMouseDownEvent();
                return;
            }

            dragDoing = true;
            RaiseHMouseDownEvent();

            mouseX = e.X;
            mouseY = e.Y;
            mouseRow = e.Row;
            mouseColumn = e.Column;
        }

        private void HalconViewer_MouseLeave(object sender, MouseEventArgs e)
        {
            dragDoing = false;
        }

        public HWindowControlWPF HWindowControl => HWindowControlWpf;

        public void ClearWindow()
        {
            HWindowControlWpf.HalconWindow.ClearWindow();
        }

        public void ZoomToRectangle(double centerX, double centerY, double width, double height, double scaleFactor = 1)
        {
            if (Math.Abs(width) < 0.000001 & Math.Abs(height) < 0.000001)
                return;

            dragDoing = false;
            //            if (BitmapSource == null)
            //                return;

            var wRatio = ActualWidth / width;
            var hRatio = ActualHeight / height;

            var scale = wRatio > hRatio ? hRatio : wRatio;
            scale = scale * scaleFactor;

            Scale = scale;

            X = (+centerX) - ActualWidth / scale / 2;
            Y = (+centerY) - ActualHeight / scale / 2;

            Refresh();
        }

        #region HWindowMouseDownEvent

        public static readonly RoutedEvent HMouseDownEvent = EventManager.RegisterRoutedEvent
            ("HMouseDown", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(HalconViewer));

        public event RoutedEventHandler HMouseDown
        {
            add { AddHandler(HMouseDownEvent, value); }
            remove { RemoveHandler(HMouseDownEvent, value); }
        }

        private void RaiseHMouseDownEvent()
        {
            var newEventArgs = new RoutedEventArgs(HMouseDownEvent);
            RaiseEvent(newEventArgs);
        }

        #endregion

    }


}