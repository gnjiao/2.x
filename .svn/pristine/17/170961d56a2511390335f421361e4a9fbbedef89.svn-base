using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class RcApiCommandController : ICommandController
    {
        public void Initialize()
        {
        }

        public void Command(MeasureEvent measureEvent)
        {
            RcApiService.Singletone.SetVar(VarName, Value);
        }

        public string VarName { get; set; }
        public int Value { get; set; }
    }
}