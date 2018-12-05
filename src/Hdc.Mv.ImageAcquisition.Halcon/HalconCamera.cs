using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows.Markup;
using HalconDotNet;
using Hdc.Diagnostics;
using Hdc.Mv.Halcon;
using Hdc.Mv.ImageAcquisition.Halcon;

namespace Hdc.Mv.ImageAcquisition
{
    [Serializable]
    [ContentProperty("ParamEntries")]
    public class HalconCamera : ICamera
    {
        public void Dispose()
        {
            HFramegrabber.Dispose();
        }

        public ImageInfo Acquisition()
        {
            var sw2 = new NotifyStopwatch("HalconCamera.Acquisition(): _hFramegrabber.GrabImage()");
            var image = HFramegrabber.GrabImage();
            sw2.Dispose();

            var imageInfo = image.ToImageInfo();

            return imageInfo;
        }

        public HImage AcquisitionOfHImage()
        {
            var image = HFramegrabber.GrabImage();
            return image;
        }

        public HImage GrabImage()
        {
            var image = HFramegrabber.GrabImage();
            return image;
        }

        public HImage GrabImageAsync()
        {
            var image = HFramegrabber.GrabImageAsync(-1.0);
            return image;
        }

        public HFramegrabber HFramegrabber { get; private set; }

        public Collection<FrameGrabberParamEntry> ParamEntries { get; set; } = new Collection<FrameGrabberParamEntry>();

        public bool Init()
        {
            int genericNumber;
            var isGenericANumber = int.TryParse(OpenFramegrabber_Generic, out genericNumber);

            HTuple genericHTuple;
            if (isGenericANumber)
            {
                genericHTuple = genericNumber;
            }
            else
            {
                genericHTuple = OpenFramegrabber_Generic;
            }


            HFramegrabber = new HFramegrabber();
            HFramegrabber.OpenFramegrabber(
                name: OpenFramegrabber_Name,
                horizontalResolution: OpenFramegrabber_HorizontalResolution,
                verticalResolution: OpenFramegrabber_VerticalResolution,
                imageWidth: OpenFramegrabber_ImageWidth,
                imageHeight: OpenFramegrabber_ImageHeight,
                startRow: OpenFramegrabber_StartRow,
                startColumn: OpenFramegrabber_StartColumn,
                field: OpenFramegrabber_Field,
                bitsPerChannel: new HTuple(OpenFramegrabber_BitsPerChannel),
                colorSpace: new HTuple(OpenFramegrabber_ColorSpace),
                generic: genericHTuple,
                externalTrigger: OpenFramegrabber_ExternalTrigger,
                cameraType: new HTuple(OpenFramegrabber_CameraType),
                device: new HTuple(OpenFramegrabber_Device),
                port: new HTuple(OpenFramegrabber_Port),
                lineIn: new HTuple(OpenFramegrabber_LineIn));

            Console.WriteLine("------------------------------------");
            foreach (var paramEntry in ParamEntries)
            {
                $"HalconCamera.SetFramegrabberParam: Name={paramEntry.Name}, Value={paramEntry.Value}, DataType={paramEntry.DataType}".WriteLineInConsoleAndDebug();
                HFramegrabber.SetFramegrabberParam(paramEntry.Name, paramEntry.Value, paramEntry.DataType);
                var value = HFramegrabber.GetFramegrabberParam(paramEntry.Name);
                $"HalconCamera.GetFramegrabberParam: Name={paramEntry.Name}, Value={value}".WriteLineInConsoleAndDebug();
            }
            Console.WriteLine("------------------------------------");
            foreach (var paramEntry in ParamEntries)
            {
//                $"HalconCamera.SetFramegrabberParam: Name={paramEntry.Name}, Value={paramEntry.Value}, DataType={paramEntry.DataType}".WriteLineInConsoleAndDebug();
//                HFramegrabber.SetFramegrabberParam(paramEntry.Name, paramEntry.Value, paramEntry.DataType);
                var value = HFramegrabber.GetFramegrabberParam(paramEntry.Name);
                $"HalconCamera.GetFramegrabberParam: Name={paramEntry.Name}, Value={value}".WriteLineInConsoleAndDebug();
            }
            Console.WriteLine("------------------------------------");
            return true;
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

        public void SetParamerter(string name, string value)
        {
            HFramegrabber?.SetFramegrabberParam(name, value);
        }

        public void SetParamerter(string name, int value)
        {
            HFramegrabber?.SetFramegrabberParam(name, value);
        }

        public object GetParamerter(string name)
        {
            var value = HFramegrabber?.GetFramegrabberParam(name);
            return value;
        }
    }
}