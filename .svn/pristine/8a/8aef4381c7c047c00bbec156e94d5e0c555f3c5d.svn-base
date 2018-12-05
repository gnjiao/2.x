using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Markup;

namespace Hdc.Measuring
{
    [Serializable]
    [ContentProperty("CalculateOperation")]
    public class CalculateDefinition
    {
        private double _standardValue;
        private double _toleranceUpper;
        private double _toleranceLower;
        public int Index { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
        
        public ICalculateOperation CalculateOperation { get; set; }

        public string ReferenceMeasureOutputName { get; set; }

        public int StationIndex { get; set; }

        public bool IsStationIndexEnabled { get; set; }

        [Obsolete]
        [Browsable(false)]
        public double ExpectValue
        {
            get { return _standardValue; }
            set { _standardValue = value; }
        }

        public double StandardValue
        {
            get { return _standardValue; }
            set { _standardValue = value; }
        }

        [Obsolete]
        [Browsable(false)]
        public double TolerancePlus
        {
            get { return _toleranceUpper; }
            set { _toleranceUpper = value; }
        }

        public double ToleranceUpper
        {
            get { return _toleranceUpper; }
            set { _toleranceUpper = value; }
        }

        [Obsolete]
        [Browsable(false)]
        public double ToleranceMinus
        {
            get { return _toleranceLower; }
            set { _toleranceLower = value; }
        }

        public double ToleranceLower
        {
            get { return _toleranceLower; }
            set { _toleranceLower = value; }
        }

        public double SystemOffsetValue { get; set; }

        public string Unit { get; set; }

        public int FloatPosition { get; set; } = 3;

        public bool IsDisabled { get; set; }

        public bool IsHidden { get; set; }

        public bool TuneEnabled { get; set; }

        public double TuneParameter { get; set; }

        public string SipNo { get; set; }

        public Collection<FixtureCorrectionDefinition> FixtureCorrectionDefinitions { get; set; } =
            new Collection<FixtureCorrectionDefinition>();

        public double IsNGOffsetUpper { get; set; }
        public double IsNGOffsetLower { get; set; }
        public double IsNG2OffsetUpper { get; set; }
        public double IsNG2OffsetLower { get; set; }
        public double IsOver20umOffsetUpper { get; set; }
        public double IsOver20umOffsetLower { get; set; }

        public double AbnormalValueLimitUpper { get; set; } = 1.0;
        public double AbnormalValueLimitLower { get; set; } = -1.0;

        public double ToleranceUpperWithAbnormalValueLimitUpper => ToleranceUpper + AbnormalValueLimitUpper;
        public double ToleranceLowerWithAbnormalValueLimitLower => ToleranceLower + AbnormalValueLimitLower;

        public Collection<CalculateLevel> CalculateLevels { get; set; } = new Collection<CalculateLevel>();
    }
}