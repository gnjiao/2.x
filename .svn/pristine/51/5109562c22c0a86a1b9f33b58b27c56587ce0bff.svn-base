using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Hdc.Boot;
using Hdc.Collections.Concurrent;
using Hdc.Collections.Generic;
using Hdc.Diagnostics;
using Hdc.Mercury;
using Hdc.Mercury.Communication;
using Hdc.Mercury.Communication.OPC.Xi;
using Hdc.Reactive.Linq;

namespace Hdc.Measuring.VinsML
{
    public class VinsMLMeasureService : IMeasureService
    {
        private readonly Subject<StationResult> _stationCompletedEvent =
            new Subject<StationResult>();

        private int _measureTagCounter;
        private bool _measureEnabled;

        public IObservable<StationResult> StationCompletedEvent => _stationCompletedEvent;

        private readonly ConcurrentDictionary<int, MeasureResult> _measureResultDictionary =
            new ConcurrentDictionary<int, MeasureResult>();

        private MeasureEvent _workpieceMeasureEvent;

        private readonly Dictionary<string, MeasureOutput> _localMeasureOutputs = new Dictionary<string, MeasureOutput>();

        private object locker = new object();

        public void Initialize(MeasureSchema measureSchema)
        {
            foreach (var initializer in measureSchema.Initializers)
            {
                initializer.Initialize();
            }

            measureSchema.WorkpieceInPositionEventController.Initialize();
            measureSchema.WorkpieceInPositionCommandController.Initialize();
            measureSchema.SensorInPositionEventController.Initialize();
            measureSchema.SensorInPositionCommandController.Initialize();
            measureSchema.StationCompletedEventController.Initialize();
            measureSchema.StationCompletedCommandController?.Initialize();
            measureSchema.StationErrorCommandController?.Initialize();
            measureSchema.WorkpieceTagService?.Initialize();

            foreach (var measureDefinition in measureSchema.MeasureDefinitions)
            {
                if (!measureDefinition.Device.IsInitialized)
                    measureDefinition.Device.Initialize();
            }

            foreach (var plugin in measureSchema.Plugins)
            {
                plugin.Initialize(measureSchema);
            }

            measureSchema.InitializeStationEventController?.Event.Subscribe(measureEvent =>
            {
                Console.WriteLine("InitializeStationEventController Event raised, at " + DateTime.Now);
                Console.WriteLine("InitializeStationCommandController Command begin, at " + DateTime.Now);
                measureSchema.InitializeStationCommandController?.Command(measureEvent);
                Console.WriteLine("InitializeStationCommandController Command end, at " + DateTime.Now);
            });

            measureSchema.WorkpieceInPositionEventController.Event.ObserveOnTaskPool().Subscribe(
                measureEvent =>
                {
                    Debug.WriteLine("WorkpieceInPositionEventSignalController.Event raised, at " +
                                    DateTime.Now);

                    lock (locker)
                    {
                        if (_measureEnabled)
                        {
                            var message = "WorkpieceInPositionEventController abnormal at " + DateTime.Now
                                           + "\n _measureEnabled == true";
                            Console.WriteLine(message);
                            return;
                        }

                        _measureEnabled = true;
                    }
                 
                    _measureResultDictionary.Clear();

                    _workpieceMeasureEvent = measureEvent;

                    if (measureSchema.WorkpieceInPositionCommandController2 == null)
                    {
                        measureSchema.WorkpieceInPositionCommandController?.Command(measureEvent);
                    }
                    else
                    {
                        measureSchema.StationIndex = measureEvent.StationIndex;
                        switch (measureEvent.StationIndex)
                        {
                            case 1:
                                measureSchema.WorkpieceInPositionCommandController?.Command(measureEvent);
                                break;
                            case 2:
                                measureSchema.WorkpieceInPositionCommandController2?.Command(measureEvent);
                                break;
                        }
                    }

                    Debug.WriteLine("WorkpieceInPositionEventSignalController.Acknowledge()" + ", at " + DateTime.Now);
                });

            measureSchema.SensorInPositionEventController.Event.ObserveOnTaskPool()
                .Subscribe(async sensorMeasureEvent =>
                {
                    Debug.WriteLine("SensorInPositionEventController.Event raised, at " + DateTime.Now);

                    if (!_measureEnabled)
                    {
                        var message = "SensorInPositionEventController abnormal at " + DateTime.Now
                                      + "\n _measureEnabled == false";
                        Console.WriteLine(message);
                        throw new InvalidOperationException(
                            "SensorInPositionEventController error, _measureEnabled is false");
                    }

                    //
                    sensorMeasureEvent.WorkpieceTag = _workpieceMeasureEvent.WorkpieceTag;
                    sensorMeasureEvent.StationIndex = _workpieceMeasureEvent.StationIndex;

                    //
                    var measureDefinition =
                        measureSchema.MeasureDefinitions.Single(p => p.PointIndex == sensorMeasureEvent.PointIndex);

                    var beforeTriggers = measureDefinition.GetBeforeTriggers();
                    foreach (var trigger in beforeTriggers)
                    {
                        trigger.Action(measureDefinition.Device, sensorMeasureEvent.PointIndex);
                    }

                    //
                    var sw = new NotifyStopwatch("MeasureAsync");
                    var measureResult = await measureDefinition.Device.MeasureAsync(sensorMeasureEvent);

                    if (measureResult.Outputs.Count > 0)
                        Console.WriteLine("measureResult[0]=" + measureResult.Outputs[0].Value);

                    measureResult.Index = measureDefinition.Index;
                    measureResult.MeasureEvent = sensorMeasureEvent.DeepClone();
                    sw.Dispose();

                    //
                    var afterTriggers = measureDefinition.GetAfterTriggers();
                    foreach (var trigger in afterTriggers)
                    {
                        trigger.Action(measureDefinition.Device, sensorMeasureEvent.PointIndex, measureResult);
                    }


                    //
                    _measureResultDictionary.AddOrUpdate(sensorMeasureEvent.PointIndex, measureResult,
                        (i, oriValue) => measureResult);
                    _measureTagCounter++;

                    //
                    var sw2 = new NotifyStopwatch("SensorInPositionCommandController.Command()");
                    measureSchema.SensorInPositionCommandController.Command(sensorMeasureEvent);
                    sw2.Dispose();

                    Console.WriteLine(
                        $"SensorInPositionEventController end:: _currentPointIndex = {sensorMeasureEvent.PointIndex}, at " +
                        DateTime.Now);
                    Console.WriteLine(
                        $"SensorInPositionEventController end:: _measureResults.Count = {_measureResultDictionary.Count}, at " +
                        DateTime.Now);
                });


            measureSchema.StationCompletedEventController.Event.ObserveOnTaskPool().Subscribe(x =>
            {
                Console.WriteLine("StationCompletedEventController.Event raised, at " + DateTime.Now);

                var measureResults = _measureResultDictionary.Select(p => p.Value).ToList();
                //_measureResultDictionary.Clear();

                _measureEnabled = false;


                // InMemoryCalculationOperations
                _localMeasureOutputs.Clear();

                int indexCounter = 99;
                foreach (var operation in measureSchema.InMemoryCalculationOperations)
                {
                    var mr = new MeasureResult() { };
                    var measureOutputs = operation.Calculate(measureResults);
                    foreach (var measureOutput in measureOutputs)
                    {
                        _localMeasureOutputs.Add(measureOutput.Name, measureOutput);
                        mr.Outputs.Add(measureOutput);
                    }
                    mr.Index = indexCounter;
                    measureResults.Add(mr);
                    indexCounter++;
                }

                //


                var stationResult = new StationResult()
                {
                    StationIndex = x.StationIndex, //measureSchema.StationIndex,
                    StationName = measureSchema.StationName,
                    StationDescription = measureSchema.StationDescription,
                    MeasureResults = measureResults,
                    MeasureTag = _measureTagCounter,
                    WorkpieceTag = _workpieceMeasureEvent.WorkpieceTag,
                    MeasureDateTime = DateTime.Now,
                };

                if (stationResult.HasError)
                {
                    measureSchema.StationErrorCommandController.Command(x);
                    var message2 = "StationErrorCommandController.Command() invoked, at " + DateTime.Now;
                    Debug.WriteLine(message2);
                    Console.WriteLine(message2);
                    return;
                }
                
                stationResult.CalculateResults = CalculateResults(measureSchema, measureResults,x.StationIndex);

                //
                if (measureSchema.StationCompletedCommandController2 == null)
                {
                    measureSchema.StationCompletedCommandController?.Command(x);
                    var message3 = "StationCompletedCommandController.Command() invoked, at " + DateTime.Now;
                    Debug.WriteLine(message3);
                    Console.WriteLine(message3);
                }
                else
                {
                    switch (x.StationIndex)
                    {
                        case 1:
                            measureSchema.StationCompletedCommandController?.Command(x);
                            var message4 = "StationCompletedCommandController.Command() invoked, at " + DateTime.Now;
                            Debug.WriteLine(message4);
                            Console.WriteLine(message4);
                            break;
                        case 2:
                            measureSchema.StationCompletedCommandController2?.Command(x);
                            var message5 = "StationCompletedCommandController2.Command() invoked, at " + DateTime.Now;
                            Debug.WriteLine(message5);
                            Console.WriteLine(message5);
                            break;
                    }
                }


                //
                foreach (var stationResultProcessor in measureSchema.StationResultProcessors)
                {
//                    var clone = stationResult.DeepClone();
                    stationResultProcessor.Process(stationResult);
                }

                foreach (var calculateResult in stationResult.CalculateResults)
                {
                    var selected = calculateResult.Definition.FixtureCorrectionDefinitions
                        .FirstOrDefault(p => p.FixtureDataCode == stationResult.FixtureDataCode);

                    if (selected != null)
                    {
                        calculateResult.FixtureCorrectionValue = selected.CorrectionValue;
                    }
                }

                ConsolePrintCalculateResults(stationResult);

                ("FixtureDataCode " + stationResult.FixtureDataCode + ", at " + DateTime.Now).WriteLineInConsoleAndDebug();
                ("WorkpieceDataCode " + stationResult.WorkpieceDataCode + ", at " + DateTime.Now).WriteLineInConsoleAndDebug();

                _stationCompletedEvent.OnNext(stationResult.DeepClone());
            });
        }

        private List<CalculateResult> CalculateResults(MeasureSchema measureSchema, List<MeasureResult> results, int stationIndex)
        {
            List<CalculateResult> calculateResults = new List<CalculateResult>();

            for (int i = 0; i < measureSchema.CalculateDefinitions.Count; i++)
            {
                var definition = measureSchema.CalculateDefinitions[i];

                if ((definition.IsStationIndexEnabled  && 
                    definition.StationIndex != measureSchema.StationIndex) ||
                    definition.StationIndex != stationIndex)
                {
                    continue;
                }

                MeasureOutput measureOutput;
                if (string.IsNullOrEmpty(definition.ReferenceMeasureOutputName))
                {
                    measureOutput = definition.CalculateOperation.Calculate(results, definition);
                }
                else
                {
                    measureOutput = _localMeasureOutputs[definition.ReferenceMeasureOutputName];
                }

                if (definition.TuneEnabled)
                {
                    measureOutput.Value = MeasureOutputValueTuner.Tune(
                        measureOutput.Value,
                        definition.StandardValue,
                        definition.ToleranceUpper,
                        definition.ToleranceLower,
                        definition.SystemOffsetValue);
                    measureOutput.IsTuned = true;
                }

                var calculateResult = new CalculateResult
                {
                    Index = i,
                    Definition = definition,
                    Output = measureOutput
                };
                calculateResult.LevelName = calculateResult.GetLevelName();

                calculateResults.Add(calculateResult);
            }
            return calculateResults;
        }

        private static void ConsolePrintCalculateResults(StationResult stationResult)
        {
            Console.WriteLine("StationResult Completed at " + DateTime.Now);
            foreach (var p in stationResult.CalculateResults)
            {
                Console.WriteLine(
                    "--CalculateResult-" + p.Definition.Name +
                    ", SipNo:" + p.Definition.SipNo +
                    ", Output:" + p.Output.Value.ToString("00.000") +
                    ", FinalValue: " + p.FinalValue.ToString("00.000") 
                    );
            }
        }
    }
}