using System;
using System.Reactive.Linq;
using Vins.ML.Domain;

namespace Hdc.Measuring
{
    [Serializable]
    public class StationWatchdogViaMqPlugin : IMeasureSchemaPlugin
    {
        public void Initialize(MeasureSchema measureSchema)
        {
            Console.WriteLine($"{nameof(StationWatchdogViaMqPlugin)}.Initialize()");
            Observable
                .Interval(TimeSpan.FromSeconds(2))
                .Subscribe(x =>
                {
                    MqInitializer.Bus.PublishAsync(new StationWatchdogMqEvent()
                    {
                        StationIndex = measureSchema.StationIndex,
                        StationName = measureSchema.StationName,
                        DateTime = DateTime.Now,

                        ConfigIndex = measureSchema.ConfigIndex,
                        ConfigName = measureSchema.ConfigName,
                        ConfigDisplayName= measureSchema.ConfigDisplayName,
                        ConfigComment= measureSchema.ConfigComment,

                    }, cfg => cfg.WithExpires(5000));
                });
        }
    }
}