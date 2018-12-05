using System;
using Hdc.Mv;

namespace Hdc.Measuring
{
    [Serializable]
    public class ActiveComputerDevicePlugin: IMeasureSchemaPlugin
    {
        readonly ActiveComputerDeviceInspectorInitializer Initializer = new ActiveComputerDeviceInspectorInitializer();

        public void Initialize(MeasureSchema measureSchema)
        {
            Initializer.DeviceIdentifier = DeviceIdentifier;
            Initializer.Initialize();
        }

        public int DeviceIdentifier { get; set; } = 0;

    }
}