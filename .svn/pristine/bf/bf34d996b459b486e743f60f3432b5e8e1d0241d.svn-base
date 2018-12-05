using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace Hdc.Measuring
{
    [Serializable]
    public class MultipleInitializerHFramegrabberProviderPlugin : IMeasureSchemaPlugin
    {
        public double GrabAsyncMaxDelay { get; set; } = -1.0;

        public int Index { get; set; }

        public HFramegrabber HFramegrabber => MultipleHalconCameraInitializer.Singleton.HalconCamerals[Index].HFramegrabber;

        public void Initialize(MeasureSchema measureSchema)
        {
            HFramegrabber.GrabImageAsync(GrabAsyncMaxDelay);
        }
    }
}
