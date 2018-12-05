using Hdc;
using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class PublishMqEventStationResultProcessor : IStationResultProcessor
    {
        public void Process(StationResult stationResult2)
        {
            var clone = stationResult2.DeepClone();
            clone.ClearCalculationOperations();
            var measureCompletedMqEvent = new StationCompletedMqEvent() {StationResult = clone };
            MqInitializer.Bus.PublishAsync(measureCompletedMqEvent, p => p.WithExpires(Expires));
            Console.WriteLine(
                $"PublishMqEventStationResultProcessor, MeasureCompletedMqEvent published, Station={clone.StationIndex}, at " +
                DateTime.Now);
        }

        /// <summary>
        /// The TTL to set in milliseconds
        /// </summary>
        public int Expires { get; set; } = 10000;
    }
}