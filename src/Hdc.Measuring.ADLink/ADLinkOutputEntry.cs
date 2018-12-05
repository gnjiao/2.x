using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class ADLinkOutputEntry
    {
        public int ChannelIndex { get; set; }
        public bool Value { get; set; }
    }
}