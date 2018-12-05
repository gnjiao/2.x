using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Windows.Markup;

namespace Hdc.Measuring
{
    [Serializable]
    [ContentProperty("CalculateDefinitions")]
    public class MeasureSchema
    {
        public int StationIndex { get; set; }

        public string StationName { get; set; }

        public string StationDescription { get; set; }

        public int ConfigIndex { get; set; }
        public string ConfigName { get; set; }
        public string ConfigDisplayName { get; set; }
        public string ConfigComment { get; set; }

        public Collection<IInitializer> Initializers { get; set; } = new Collection<IInitializer>();

        public Collection<IMeasureSchemaPlugin> Plugins { get; set; } = new Collection<IMeasureSchemaPlugin>();

        public Collection<MeasureDefinition> MeasureDefinitions { get; set; } = new Collection<MeasureDefinition>();

        public Collection<CalculateDefinition> CalculateDefinitions { get; set; } =
            new Collection<CalculateDefinition>();

        public IEventController InitializeStationEventController { get; set; }
        public ICommandController InitializeStationCommandController { get; set; }

        public IEventController WorkpieceInPositionEventController { get; set; }
        public ICommandController WorkpieceInPositionCommandController { get; set; }
        public ICommandController WorkpieceInPositionCommandController2 { get; set; }

        public IEventController SensorInPositionEventController { get; set; }
        public ICommandController SensorInPositionCommandController { get; set; }

        public IEventController StationCompletedEventController { get; set; }
        public ICommandController StationCompletedCommandController { get; set; }
        public ICommandController StationCompletedCommandController2 { get; set; }
        public ICommandController StationErrorCommandController { get; set; }

        [Obsolete]  
        public IWorkpieceTagService WorkpieceTagService { get; set; }

        public ICollection<IStationResultProcessor> StationResultProcessors { get; set; } =
            new Collection<IStationResultProcessor>();

        public Collection<ICalculationOperationPlus> InMemoryCalculationOperations { get; set; } =
            new Collection<ICalculationOperationPlus>();
    }
}