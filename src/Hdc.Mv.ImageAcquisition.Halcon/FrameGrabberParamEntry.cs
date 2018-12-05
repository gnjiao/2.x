using System;

namespace Hdc.Mv.ImageAcquisition.Halcon
{
    [Serializable]
    public class FrameGrabberParamEntry
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public FrameGrabberParamEntryDataType DataType { get; set; } = FrameGrabberParamEntryDataType.String;

        public FrameGrabberParamEntry()
        {
        }

        public FrameGrabberParamEntry(string name, string value, FrameGrabberParamEntryDataType dataType = FrameGrabberParamEntryDataType.String)
        {
            Name = name;
            Value = value;
            DataType = dataType;
        }
    }
}