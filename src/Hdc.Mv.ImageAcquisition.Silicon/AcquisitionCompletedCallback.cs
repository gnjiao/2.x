using System;

namespace Hdc.Mv.ImageAcquisition
{
    [Serializable]
    public delegate void AcquisitionCompletedCallback(Int64 msg, ImageInfo imageInfo);
}