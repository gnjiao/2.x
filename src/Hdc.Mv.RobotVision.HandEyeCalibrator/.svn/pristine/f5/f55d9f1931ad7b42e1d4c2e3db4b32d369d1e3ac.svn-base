using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading;
using System.Windows.Threading;
using HalconDotNet;
using Hdc.Mv.ImageAcquisition;
using Hdc.Mv.ImageAcquisition.Halcon;

namespace Hdc.Mv.RobotVision.HandEyeCalibrator
{
    public class GrabService : IGrabService
    {
        private readonly HalconCamera _halconFrameGrabber = new HalconCamera();
//        private readonly HalconFrameGrabber2 _halconFrameGrabber = new HalconFrameGrabber2();

        public Config Config => ConfigProvider.Config;
        public Subject<Unit> _InitializedEvent = new Subject<Unit>();

        public void Initialize()
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                _halconFrameGrabber.OpenFramegrabber_Name = Config.OpenFramegrabber_Name;
                _halconFrameGrabber.OpenFramegrabber_Generic = Config.OpenFramegrabber_Generic;
                _halconFrameGrabber.OpenFramegrabber_CameraType = Config.OpenFramegrabber_CameraType;

                if (Config.OpenFramegrabber_Name == "GigEVision")
                {
                    _halconFrameGrabber.ParamEntries.Add(new FrameGrabberParamEntry("AcquisitionMode", "SingleFrame"));
                    // SingleFrame, Continuous,
                    _halconFrameGrabber.ParamEntries.Add(new FrameGrabberParamEntry("AcquisitionFrameRate", "1.0",
                        FrameGrabberParamEntryDataType.Double));
                    _halconFrameGrabber.ParamEntries.Add(new FrameGrabberParamEntry("start_async_after_grab_async",
                        "disable"));
                    _halconFrameGrabber.ParamEntries.Add(new FrameGrabberParamEntry("ExposureMode", "Timed"));
                    _halconFrameGrabber.ParamEntries.Add(new FrameGrabberParamEntry("ExposureAuto", "Off"));
                    _halconFrameGrabber.ParamEntries.Add(new FrameGrabberParamEntry("ExposureTime", Config.ExposureTime.ToString(),
                        FrameGrabberParamEntryDataType.Double));
                }

                _halconFrameGrabber.Init();

                _InitializedEvent.OnNext(new Unit());
                //
                //            _halconFrameGrabber.GrabbedEvent.Subscribe(
                //                x =>
                //                {
                //                    return;
                //
                //                    Application.Current.Dispatcher.BeginInvoke(
                //                        new Action(() =>
                //                        {
                //                            HalconViewer.ClearWindow();
                //                            HalconViewer.Image?.Dispose();
                //
                //                            HalconViewer.Image = x;
                //
                //                            if (_oriInspector == null) return;
                //                            var result = _oriInspector.Inspect(x);
                //
                //                            DefinitionsCircleIndicators.Clear();
                //                            ResultsCircleIndicators.Clear();
                //
                //                            Show_CircleSearchingDefinitions(result.GetCircleSearchingDefinitions());
                //                            Show_CircleSearchingResults(result.Circles);
                //                        }));
                //                });


                Dispatcher.Run();
            }));

            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();
        }

        public void EnableGrabContinue()
        {
            _halconFrameGrabber.SetParamerter("start_async_after_grab_async", "enable");
            _halconFrameGrabber.ParamEntries.Add(new FrameGrabberParamEntry("AcquisitionMode", "Continuous"));
            // SingleFrame, Continuous,
        }

        public void DisableGrabContinue()
        {
            _halconFrameGrabber.SetParamerter("start_async_after_grab_async", "disable");
            _halconFrameGrabber.ParamEntries.Add(new FrameGrabberParamEntry("AcquisitionMode", "SingleFrame"));
            // SingleFrame, Continuous,
        }

        public HImage Grab()
        {
            var image = _halconFrameGrabber.GrabImage();
            return image;
        }

        public HImage GrabAsync()
        {
            var image = _halconFrameGrabber.GrabImageAsync();
            return image;
        }

        public void Uninitialize()
        {
            if (Config.OpenFramegrabber_Name == "GigEVision")
            {
                _halconFrameGrabber?.SetParamerter("start_async_after_grab_async", "disable");
            }
        }

        public object GetParamerter(string paraName)
        {
            return _halconFrameGrabber.GetParamerter(paraName);
        }

        public void SetParamerter(string paraName, string value)
        {
            _halconFrameGrabber.SetParamerter(paraName, value);
        }

        public void SetParamerter(string paraName, int value)
        {
            _halconFrameGrabber.SetParamerter(paraName, value);
        }

        public IObservable<Unit> InitializedEvent => _InitializedEvent;
    }
}