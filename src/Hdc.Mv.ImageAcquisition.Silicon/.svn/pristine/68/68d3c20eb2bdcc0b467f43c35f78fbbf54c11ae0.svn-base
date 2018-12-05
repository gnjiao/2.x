using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hdc.Mv.ImageAcquisition
{
    [Obsolete]
    public class SiliconCameraFactory
    {
        public SiliconCameraFactory(int boardCount, GrabberInfo[] grabberInfos)
        {
            var ret = SiliconGrabberInterop.Initialize(boardCount, grabberInfos);
            if (ret != 0)
                throw new SiliconException("SiliconGrabberInterop.Initialize error");
        }

        public IFrameGrabber CreateCamera(int boardIndex, int camPortIndex)
        {
            var SiliconCamera = new SiliconFrameGrabber
            {
                BoardIndex = boardIndex,
                CamPortIndex = camPortIndex
            };

            SiliconCamera.Initialize();
            //SiliconGrabberInterop.RegisterCallback(boardIndex, camPortIndex, SiliconFrameGrabber.AcquisitionCompletedCallback);
            return SiliconCamera;
        }
    }
}