using System;
using Hdc.Mercury;
using Hdc.Reactive;

namespace Hdc.Mercury
{
    public class RootDeviceGroupProvider
    {
        private static readonly RetainedSubject<IDeviceGroup> _rootDeviceGroupInitializedEvent =
            new RetainedSubject<IDeviceGroup>();

        public static IDeviceGroup RootDeviceGroup
        {
            get { return _rootDeviceGroupInitializedEvent.Value; }
            set { _rootDeviceGroupInitializedEvent.Value = value; }
        }

        public static IObservable<IDeviceGroup> RootDeviceGroupInitializedEvent
        {
            get { return _rootDeviceGroupInitializedEvent; }
        }
    }
}