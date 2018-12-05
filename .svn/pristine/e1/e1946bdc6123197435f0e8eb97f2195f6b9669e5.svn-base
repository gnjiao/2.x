using System.Threading.Tasks;

namespace Hdc.Measuring
{
    public static class MeasureDeviceExtensions
    {
        public static Task<MeasureResult> MeasureAsync(this IMeasureDevice measureDevice, MeasureEvent info)
        {
            return Task.Run(() => measureDevice.Measure(info));
        }
    }
}