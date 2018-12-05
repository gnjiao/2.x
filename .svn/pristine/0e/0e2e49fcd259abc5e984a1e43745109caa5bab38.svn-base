using System;

namespace Hdc.Measuring
{
    public static class MeasureOutputValueTuner
    {
        public static double Tune(double value, double standardValue, double toleranceUpper, double toleranceLower, double systemOffsetValue = 0)
        {
            //
            var finalValue = value + systemOffsetValue;
            double limitUpper = standardValue + toleranceUpper;
            double limitLower = standardValue + toleranceLower;

            //
            if ((finalValue >= limitUpper) && finalValue <= (limitUpper + 0.031))
            {
                return value - 0.021;
            }

            if ((finalValue <= limitLower) && finalValue >= (limitLower - 0.031))
            {
                return value + 0.021;
            }

            return value;
        }
    }
}