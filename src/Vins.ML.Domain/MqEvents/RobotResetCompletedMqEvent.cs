namespace Vins.ML.Domain
{
    public class RobotResetCompletedMqEvent
    {
        public int Index { get; set; }

        public RobotResetCompletedMqEvent()
        {
        }

        public RobotResetCompletedMqEvent(int index)
        {
            Index = index;
        }
    }
}