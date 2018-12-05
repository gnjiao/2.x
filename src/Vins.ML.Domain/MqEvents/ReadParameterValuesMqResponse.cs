using System;
using System.Collections.Generic;

namespace Vins.ML.Domain
{
    public class ReadParameterValuesMqResponse
    {
        public List<ParameterValueChangedMqEvent> ChangedMqEvents { get; set; } =
            new List<ParameterValueChangedMqEvent>();
    }
}