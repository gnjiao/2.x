using System;
using System.Collections.ObjectModel;
using System.Linq;
using Hdc.Collections.Generic;

namespace Hdc.Mv.ImageAcquisition
{
    [Serializable]
    public class SiliconInitializer : IFrameGrabberInitializer
    {
        public SiliconInitializer()
        {
            GrabberInfos = new Collection<GrabberInfo>();
        }

        public SiliconInitializer(params GrabberInfo[] grabberInfos)
            : this()
        {
            GrabberInfos.AddRange(grabberInfos);
        }

        public void Initialize()
        {
            GrabberInfo[] grabberInfos = GrabberInfos.ToArray();
            var ret = SiliconGrabberInterop.Initialize(GrabberInfos.Count, grabberInfos);
            if (ret != 0)
                throw new SiliconException("SiliconGrabberInterop.Initialize error");
        }

        public Collection<GrabberInfo> GrabberInfos { get; set; }

        public void Dispose()
        {
            var ret = SiliconGrabberInterop.Uninitialize();
            if (ret != 0)
                throw new SiliconException("SiliconGrabberInterop.Initialize error");
        }
    }
}