using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class SpelEvent
    {
        public int EventNumber { get; set; } 
        public string Message { get; set; }
    }
}