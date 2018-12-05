using System;
using Extreme.Mathematics.SignalProcessing;

namespace Hdc.Measuring
{
    [Serializable]
    public class CalculateResult
    {
        public int Index { get; set; }
        public int DisplayIndex => Index + 1;
        public MeasureOutput Output { get; set; }
        public CalculateDefinition Definition { get; set; }
        public MeasureValidity Validity { get; set; }
        public double Deviation => FinalValue - Definition.StandardValue;
        public double FinalValue => Output.Value + Definition.SystemOffsetValue + FixtureCorrectionValue;
        public double LimitUpper => Definition.StandardValue + Definition.ToleranceUpper;
        public double LimitLower => Definition.StandardValue + Definition.ToleranceLower;
        public double FixtureCorrectionValue { get; set; }

        [Obsolete("Use IsNG2 instead")]
        public bool IsNG
            => (FinalValue > (Definition.StandardValue + Definition.ToleranceUpper + 0.0005 + Definition.IsNGOffsetUpper)) ||
               (FinalValue < (Definition.StandardValue + Definition.ToleranceLower - 0.0005 + Definition.IsNGOffsetLower)) ||
            double.IsNaN(FinalValue);

        public bool IsNG2
            => (FinalValue > (Definition.StandardValue + Definition.ToleranceUpper + 0.005 + Definition.IsNG2OffsetUpper)) ||
               (FinalValue < (Definition.StandardValue + Definition.ToleranceLower - 0.005 + Definition.IsNG2OffsetLower)) ||
            double.IsNaN(FinalValue);

        public bool IsNG2Upper
            => (FinalValue > (Definition.StandardValue + Definition.ToleranceUpper + 0.005 + Definition.IsNG2OffsetUpper));

        public bool IsNG2Lower
            => (FinalValue < (Definition.StandardValue + Definition.ToleranceLower - 0.005 + Definition.IsNG2OffsetLower));

        public bool IsOver20um
            => (FinalValue > (Definition.StandardValue + Definition.ToleranceUpper + 0.020 + 0.005 + Definition.IsOver20umOffsetUpper)) ||
               (FinalValue < (Definition.StandardValue + Definition.ToleranceLower - 0.020 - 0.005 + Definition.IsOver20umOffsetLower)) ||
            double.IsNaN(FinalValue);

        public bool IsOver20umUpper
            => (FinalValue > (Definition.StandardValue + Definition.ToleranceUpper + 0.020 + 0.005 + Definition.IsOver20umOffsetUpper));

        public bool IsOver20umLower
            => (FinalValue < (Definition.StandardValue + Definition.ToleranceLower - 0.020 - 0.005 + Definition.IsOver20umOffsetLower));

        public double DeviationOverTolerance
        {
            get
            {
                if (FinalValue > LimitUpper)
                    return FinalValue - LimitUpper;
                if (FinalValue < LimitLower)
                    return FinalValue - LimitLower;
                return 0;
            }
        }

        public bool IsAbnormal
        {
            get
            {
                var isAbnormal = FinalValue.CheckIsAbnormalValue(
                        Definition.StandardValue,
                        Definition.ToleranceUpper + Definition.AbnormalValueLimitUpper,
                        Definition.ToleranceLower + Definition.AbnormalValueLimitLower);
                return isAbnormal;
            }
        }

        public string GetLevelName()
        {
            foreach (var calculateLevel in Definition.CalculateLevels)
            {
                if (!calculateLevel.Enabled)
                    continue;

                if (FinalValue <= calculateLevel.Max &&
                    FinalValue >= calculateLevel.Min)
                    return calculateLevel.Name;
            }
            return null;
        }

        public string LevelName { get; set; }
    }
}