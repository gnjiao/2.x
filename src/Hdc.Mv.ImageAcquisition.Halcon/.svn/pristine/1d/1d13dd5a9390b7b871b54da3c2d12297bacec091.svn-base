using System;
using System.Collections.ObjectModel;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using System.Windows.Markup;
using HalconDotNet;
using Hdc.Mv.Halcon;

namespace Hdc.Mv.ImageAcquisition.Halcon
{
    [Serializable]
    [ContentProperty("ParamEntries")]
    public class HalconFrameGrabber2 : IDisposable
    {
        private HFramegrabber _hFramegrabber;
        private readonly Subject<HImage> _grabbedEvent = new Subject<HImage>();
        private HalconAPI.HFramegrabberCallback _transferEndCallback;

        public int Index { get; set; }
        public string Name { get; set; }

        public Collection<FrameGrabberParamEntry> ParamEntries { get; set; } = new Collection<FrameGrabberParamEntry>();

        public void Initialize()
        {
            _hFramegrabber = new HFramegrabber();
            _hFramegrabber.OpenFramegrabber(
                name: OpenFramegrabber_Name,
                horizontalResolution: OpenFramegrabber_HorizontalResolution,
                verticalResolution: OpenFramegrabber_VerticalResolution,
                imageWidth: OpenFramegrabber_ImageWidth,
                imageHeight: OpenFramegrabber_ImageHeight,
                startRow: OpenFramegrabber_StartRow,
                startColumn: OpenFramegrabber_StartColumn,
                field: OpenFramegrabber_Field,
                bitsPerChannel: OpenFramegrabber_BitsPerChannel,
                colorSpace: OpenFramegrabber_ColorSpace,
                generic: OpenFramegrabber_Generic,
                externalTrigger: OpenFramegrabber_ExternalTrigger,
                cameraType: OpenFramegrabber_CameraType,
                device: OpenFramegrabber_Device,
                port: OpenFramegrabber_Port,
                lineIn: OpenFramegrabber_LineIn);

            foreach (var paramEntry in ParamEntries)
            {
                switch (paramEntry.DataType)
                {
                    case FrameGrabberParamEntryDataType.String:
                        _hFramegrabber.SetFramegrabberParam(paramEntry.Name, paramEntry.Value);
                        break;
                    case FrameGrabberParamEntryDataType.Int32:
                        var intValue = Int32.Parse(paramEntry.Value);
                        _hFramegrabber.SetFramegrabberParam(paramEntry.Name, intValue);
                        break;
                    case FrameGrabberParamEntryDataType.Double:
                        var doubleValue = Double.Parse(paramEntry.Value);
                        _hFramegrabber.SetFramegrabberParam(paramEntry.Name, doubleValue);
                        break;
                    default:
                        throw new Exception("FrameGrabberParamEntryDataType not supported");
                }
            }

            _transferEndCallback = this.TransferEndCallback;

            var callbackFunction = Marshal.GetFunctionPointerForDelegate(_transferEndCallback);

            _hFramegrabber.SetFramegrabberCallback(FrameGrabberCallbackType.transfer_end.ToString()
                , callbackFunction, IntPtr.Zero);
        }

        // ReSharper disable InconsistentNaming
        public string OpenFramegrabber_Name { get; set; } = "File";
        public int OpenFramegrabber_HorizontalResolution { get; set; } = 1;
        public int OpenFramegrabber_VerticalResolution { get; set; } = 1;
        public int OpenFramegrabber_ImageWidth { get; set; } = 0;
        public int OpenFramegrabber_ImageHeight { get; set; } = 0;
        public int OpenFramegrabber_StartRow { get; set; } = 0;
        public int OpenFramegrabber_StartColumn { get; set; } = 0;
        public string OpenFramegrabber_Field { get; set; } = "default";
        public int OpenFramegrabber_BitsPerChannel { get; set; } = -1;
        public string OpenFramegrabber_ColorSpace { get; set; } = "default";
        public string OpenFramegrabber_Generic { get; set; } = "-1";
        public string OpenFramegrabber_ExternalTrigger { get; set; } = "default";
        public string OpenFramegrabber_CameraType { get; set; } = "default";
        public string OpenFramegrabber_Device { get; set; } = "default";
        public int OpenFramegrabber_Port { get; set; } = -1;
        public int OpenFramegrabber_LineIn { get; set; } = -1;
        // ReSharper restore InconsistentNaming

        public IObservable<HImage> GrabbedEvent => _grabbedEvent;

        public void Trigger()
        {
            Trigger(() => { });
        }

        public void Trigger(Action beforeTriggerAction)
        {
            beforeTriggerAction();
            _hFramegrabber.GrabImageStart(-1);
            _hFramegrabber.GrabImage();
        }

        public int Tag { get; set; }

        // User specific callback function
        public int TransferEndCallback(IntPtr handle, IntPtr user_context, IntPtr context)
        {
            var image = _hFramegrabber.GrabImageAsync(-1);
            var imageInfo = image.ToImageInfo();
            var grabInfo = new GrabInfo() { ImageInfo = imageInfo };
            _grabbedEvent.OnNext(image);
            Tag++;
            return 0;
        }

        public void SetParamerter(string name, string value)
        {
            _hFramegrabber?.SetFramegrabberParam(name, value);
        }

        public void SetParamerter(string name, int value)
        {
            _hFramegrabber?.SetFramegrabberParam(name, value);
        }

        public object GetParamerter(string name)
        {
           var value = _hFramegrabber?.GetFramegrabberParam(name);
            return value;
        }

        public void Dispose()
        {
            _hFramegrabber.Dispose();
        }

        public HFramegrabber HFramegrabber
        {
            get { return _hFramegrabber; }
        }
    }
}