using System;
using Hdc.FA.KeyenceLasers.LJV7;

namespace Hdc.Measuring
{
    [Serializable]
    public class LjvInitializer: IInitializer
    {
        public void Initialize()
        {
            NativeMethods.LJV7IF_Initialize();
            NativeMethods.LJV7IF_UsbOpen(0);
        }
    }
}