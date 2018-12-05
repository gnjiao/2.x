namespace Hdc.Mv.ImageAcquisition.Halcon
{
    public enum FrameGrabberCallbackType
    {
        transfer_end,
        device_lost,
        callback_queue_overflow,
        AcquisitionTrigger,
        AcquisitionStart,
        AcquisitionEnd,
        ExposureStart,
        ExposureEnd,
    }
}