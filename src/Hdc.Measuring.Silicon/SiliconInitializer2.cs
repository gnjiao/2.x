using System;
using System.Collections.ObjectModel;
using System.Linq;
using Hdc.Mv.ImageAcquisition;

namespace Hdc.Measuring
{
    [Serializable]
    public class SiliconInitializer2: IInitializer
    {
        public void Initialize()
        {
            GrabberInfo[] grabberInfos = GrabberInfos.ToArray();
            var ret = SiliconGrabberInterop.Initialize(GrabberInfos.Count, grabberInfos);
            if (ret != 0)
                throw new SiliconException("SiliconGrabberInterop.Initialize error");
        }

        public Collection<GrabberInfo> GrabberInfos { get; set; } = new Collection<GrabberInfo>();
    }
}