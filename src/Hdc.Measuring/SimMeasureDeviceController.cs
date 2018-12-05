/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace Hdc.Measuring
{
    [Serializable]
    public class SimMeasureDeviceController : IMeasureDeviceController
    {
        private MeasureSchema _measureSchema;

        private readonly Subject<MeasureDeviceResult> _allMeasureDataCompletedEvent =
            new Subject<MeasureDeviceResult>();

        public IMeasureDevice MeasureDevice => _measureSchema.Device;

        public void Initialize(MeasureSchema measureSchema)
        {
            _measureSchema = measureSchema;
            _measureSchema.Device.Initialize();
        }

        public IObservable<MeasureDeviceResult> AllMeasureDataCompletedEvent => _allMeasureDataCompletedEvent;

        public void TriggerWorkpieceLocatingReady()
        {
            IList<MeasureResult> mrc = new List<MeasureResult>();
            for (int i = 0; i < _measureSchema.MeasureCount; i++)
            {
                var triggers = _measureSchema.Triggers
                    .Where(trigger => trigger.TargetIndex == i || trigger.TargetIndex == -1).ToList();
                var beforeTriggers = triggers
                    .Where(trigger => trigger.TriggerType == TriggerType.Before).ToList();
                var afterTriggers = triggers
                    .Where(trigger => trigger.TriggerType == TriggerType.After).ToList();

                foreach (var trigger in beforeTriggers)
                {
                    trigger.Action(MeasureDevice, i);
                }

                var mr = MeasureDevice.Measure();

                foreach (var trigger in afterTriggers)
                {
                    trigger.Action(MeasureDevice, i);
                }

                mrc.Add(mr);
            }

            var mdr = new MeasureDeviceResult()
            {
                WorkpieceTag = 999,
                MeasureDatas = mrc,
            };

            _allMeasureDataCompletedEvent.OnNext(mdr);
        }
    }
}*/