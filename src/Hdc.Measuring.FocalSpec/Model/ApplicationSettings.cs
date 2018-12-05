using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Markup;
using Newtonsoft.Json;

namespace Hdc.Measuring
{
    /// <summary>
    /// Stores application settings that can be changed at run-time. Singleton instance.
    /// </summary>
    [Serializable]
    public sealed class ApplicationSettings
    {
        /// <summary>
        /// Gets or sets the LED pulse width in µs.
        /// </summary>
        public int LedPulseWidth { get; set; }

        /// <summary>
        /// Gets or sets the max. LED pulse width in µs. This is used only if the HW supports Automatic Gain Control (AGC).
        /// </summary>
        public int MaxLedPulseWidth { get; set; }

        /// <summary>
        /// Gets or sets the frequency in Hz.
        /// </summary>
        public int Freq { get; set; }

        /// <summary>
        /// Gets or sets the flag indicating whether external pulsing is enabled or not.
        /// </summary>
        public bool IsExternalPulsingEnabled { get; set; }

        /// <summary>
        /// Gets or sets the IPv4 address of the sensor. If null, auto-ip shall be used.
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// Backing-field of <code>ZCalibrationFile</code>.
        /// </summary>
        private string _zCalibrationFile;

        /// <summary>
        /// Gets or sets the Z calibration file.
        /// </summary>
        public string ZCalibrationFile
        {
            get { return _zCalibrationFile; }
            set
            {
                if (value != null)
                {
                    double min, max, avgGain;

                    // Due to nature of the optics, we need to calculate average pixel width [mm].
                    GetCalibrationProperties(value, 0, Defines.SensorWidth - 1, out min, out max, out avgGain);

                    AveragePixelHeight = avgGain;
                }

                _zCalibrationFile = value;
            }
        }

        /// <summary>
        /// Backing-field of <code>XCalibrationFile</code>.
        /// </summary>
        private string _xCalibrationFile;

        /// <summary>
        /// Gets or sets the X calibration file.
        /// </summary>
        public string XCalibrationFile
        {
            get { return _xCalibrationFile; }
            set
            {
                if (value != null)
                {
                    double min, max, avgGain;

                    // Due to nature of the optics, we need to calculate actual min. X [mm], max. X [mm] and average pixel width [mm] from the calibration data of the sensor at hand.
                    GetCalibrationProperties(value, 0, Defines.SensorWidth - 1, out min, out max, out avgGain);

                    OpticalProfileMinX = min;
                    OpticalProfileMaxX = max;
                    AveragePixelWidth = avgGain;
                }

                _xCalibrationFile = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of the window in [mm].
        /// </summary>
        public int UiWindowHeight { get; set; }

        /// <summary>
        /// Gets the average pixel width in [mm].
        /// </summary>
        /// <remarks>
        /// Due to the nature of the optics, pixel width is not constant in LCI1600. In LCI400 and LCI1200, it is.
        /// </remarks>
        public double AveragePixelWidth { get; set; }

        /// <summary>
        /// Gets the average pixel height [mm].
        /// </summary>
        public double AveragePixelHeight { get; set; }

        /// <summary>
        /// Gets or sets the flag indicating whether AGC is enabled or disabled. For systems not supporting AGC, this is always false.
        /// </summary>
        public bool IsAgcEnabled { get; set; }

        /// <summary>
        /// Gets or sets the AGC target intensity.
        /// </summary>
        public float AgcTargetIntensity { get; set; }

        /// <summary>
        /// Gets the flag indicating whether the Z calibration data is set or not.
        /// </summary>
        [JsonIgnore]
        public bool IsZCalibrationDataSet
        {
            get { return !string.IsNullOrEmpty(ZCalibrationFile); }
        }

        /// <summary>
        /// Gets the flag indicating whether the X calibration data is set or not.
        /// </summary>
        [JsonIgnore]
        public bool IsXCalibrationDataSet
        {
            get { return !string.IsNullOrEmpty(XCalibrationFile); }
        }

        /// <summary>
        /// Gets the min. x of the optical profile in [mm].
        /// </summary>
        [JsonIgnore]
        public double OpticalProfileMinX { get; private set; }

        /// <summary>
        /// Gets the min. x of the optical profile in [mm].
        /// </summary>
        [JsonIgnore]
        public double OpticalProfileMaxX { get; private set; }

        /// <summary>
        /// Singleton instance.
        /// </summary>
        private static volatile ApplicationSettings _instance;

        /// <summary>
        /// Protects from concurrent object instantiation.
        /// </summary>
        private static readonly object InstanceLock = new object();

        /// <summary>
        /// Assings default settings.
        /// </summary>
        public ApplicationSettings()
        {
            LedPulseWidth = Defines.DefaultPulseWidth;
            MaxLedPulseWidth = Defines.DefaultMaxPulseWidth;
            Freq = Defines.DefaultTriggerFrequency;
            IpAddress = Defines.DefaultIpAddress;
            ZCalibrationFile = Defines.DefaultZCalibrationFile;
            XCalibrationFile = Defines.DefaultXCalibrationFile;
            UiWindowHeight = Defines.DefaultUiWindowSize;
            IsAgcEnabled = Defines.DefaultAgcState;
            AgcTargetIntensity = Defines.DefaultAgcTargetIntensity;
        }

        /// <summary>
        /// Calculates min. and max. calibrated values from the calibration data given.
        /// </summary>
        /// <param name="calibrationFile">Full path to the calibration data.</param>
        /// <param name="varMin">Calibration polynomial min. parameter value.</param>
        /// <param name="varMax">Calibration polynomial max. parameter value.</param>
        /// <param name="calibMin">Min. calibrated value found.</param>
        /// <param name="calibMax">Max. calibrated value found.</param>
        /// <param name="avgGain">Average gain value among polynomials.</param>
        private void GetCalibrationProperties(string calibrationFile, int varMin, int varMax, out double calibMin, out double calibMax, out double avgGain)
        {
            calibMin = double.MaxValue;
            calibMax = double.MinValue;

            string[] lines = File.ReadAllLines(calibrationFile);
            List<double> gain = new List<double>();

            foreach (string line in lines)
            {
                List<double> coeffs = line.Split(';').Skip(1).Select(s => double.Parse(s, CultureInfo.InvariantCulture)).ToList();

                gain.Add(coeffs[1]);

                double minCandidate = Math.Min(Horner(coeffs, varMin), Horner(coeffs, varMax));
                double maxCandidate = Math.Max(Horner(coeffs, varMin), Horner(coeffs, varMax));

                calibMin = minCandidate < calibMin ? minCandidate : calibMin;
                calibMax = maxCandidate > calibMax ? maxCandidate : calibMax;
            }

            avgGain = gain.Average();
        }

        /// <summary>
        /// Horner polynomial evaluation routine at given x.
        /// </summary>
        /// <param name="coeffs">Polynomial coefficients: 0 degree, 1st degree, ..., nth degree.</param>
        /// <param name="x">Parameter value.</param>
        /// <returns>Value of the polynomial at x.</returns>
        private double Horner(List<double> coeffs, double x)
        {
            double s = 0.0f;

            for (int i = coeffs.Count-1; i >= 0; i--)
            {
                s = s * x + coeffs[i];
            }

	        return s;
        }

        /// <summary>
        /// Saves settings to file.
        /// </summary>
        public void SaveToFile()
        {
            string json = JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);

            File.WriteAllText(Defines.ApplicationSettingsFile, json);
        }

        /// <summary>
        /// Loads settings from file.
        /// </summary>
        /// <returns>New MainViewSettings object initialized from the read settings.</returns>
        public static ApplicationSettings LoadFromFile()
        {
            if (_instance == null)
            {
                lock (InstanceLock)
                {
                    if (_instance == null)
                    {
                        if (!File.Exists(Defines.ApplicationSettingsFile))
                        {
                            _instance = new ApplicationSettings();
                        }
                        else
                        {
                            string json = File.ReadAllText(Defines.ApplicationSettingsFile);

                            _instance = JsonConvert.DeserializeObject<ApplicationSettings>(json);
                        }
                    }
                }
            }

            return _instance;
        }
    }
}
