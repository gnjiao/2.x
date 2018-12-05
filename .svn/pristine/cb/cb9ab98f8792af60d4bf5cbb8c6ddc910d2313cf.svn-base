using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class MockCommandController : ICommandController
    {
        public void Initialize()
        {
        }

        public void Command(MeasureEvent measureEvent)
        {
            Console.WriteLine("MockCommandSignalController.Command: " + Message);
        }

        public string Message { get; set; }
    }
}