// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BatchConfigurationDto.cs" company="FocalSpec Oy">
//   FocalSpec Oy 2016-
// </copyright>
// <summary>
//   Batch settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Hdc.Measuring
{
    /// <summary>
    /// Batch settings.
    /// </summary>
    [Serializable]
    public class BatchConfiguration
    {
        /// <summary>
        /// Backing field for TriggerMode.
        /// </summary>
        private TriggerMode _triggerMode;

        /// <summary>
        /// Backing field for TriggerFrequency.
        /// </summary>
        private int _triggerFrequency;

        /// <summary>
        /// Backing field for ScanStepLength.
        /// </summary>
        private double _scanStepLength;

        /// <summary>
        /// Sets default values.
        /// </summary>
        public BatchConfiguration()
        {
            TriggerMode = Defines.DefaultBatchTriggerMode;
            LineSpeed = Defines.DefaultLineSpeedInBatch;
            ScanStepLength = Defines.DefaultScanStepLength;
            BatchLength = Defines.DefaultBatchLength;
        }

        /// <summary>
        /// Gets or sets the length [mm] of the scan step.
        /// </summary>
        public double ScanStepLength
        {
            get
            {
                switch (TriggerMode)
                {
                    case TriggerMode.Internal:
                        return LineSpeedMmPerS / TriggerFrequency;
                    case TriggerMode.External:
                        return _scanStepLength;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set { _scanStepLength = value; }
        }

        /// <summary>
        /// Gets or sets the line speed [m/min]. Eligible only if TriggerMode == Internal.
        /// </summary>
        public double LineSpeed { get; set; }

        /// <summary>
        /// Gets line speed in [mm/s].
        /// </summary>
        public double LineSpeedMmPerS
        {
            get
            {
                return LineSpeed / 60.0d * 1000.0d;
            }
        }

        /// <summary>
        /// Gets or sets the internal trigger frequency [Hz]. This should be used only if TriggerMode == Internal.
        /// </summary>
        public int TriggerFrequency
        {
            get { return _triggerFrequency; }
            set
            {
                if (TriggerMode == TriggerMode.External && value > Defines.ExternalTriggering)
                {
                    throw new ArgumentException("Trigger frequency must be 0 (Defines.ExternalTriggering) when using external triggering.");
                }
                _triggerFrequency = value;
            }
        }

        /// <summary>
        /// Gets or sets the trigger mode.
        /// </summary>
        public TriggerMode TriggerMode
        {
            get { return _triggerMode; }
            set
            {
                _triggerMode = value;
                if (_triggerMode == TriggerMode.External)
                {
                    TriggerFrequency = Defines.ExternalTriggering;
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of profiles in the batch.
        /// </summary>
        public int BatchLength { get; set; }

        /// <summary>
        /// Makes a deep copy of this object.
        /// </summary>
        /// <returns>
        /// A copy of this object.
        /// </returns>
        public BatchConfiguration Clone()
        {
            return new BatchConfiguration()
            {
                TriggerMode = TriggerMode,
                ScanStepLength = ScanStepLength,
                TriggerFrequency = TriggerFrequency,
                LineSpeed = LineSpeed,
                BatchLength = BatchLength
            };
        }
    }
}