using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class Ad7230MeasureCompletedForRobotCommandController : ICommandController
    {
        public void Initialize()
        {
        }

        public void Command(MeasureEvent measureEvent)
        {
            AD7230Service.Singletone.MeasureDataCompletedForRobot();
        }
    }
}