// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CameraManager.cs" company="FocalSpec Ltd">
// FocalSpec Ltd 2016-
// </copyright>
// <summary>
// Controls FocalSpec sensor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FocalSpec.FsApiNet.Model;

namespace Hdc.Measuring
{
    /// <summary>
    /// Handles received profiles.
    /// </summary>
    /// <param name="profile">Profile to handle.</param>
    public delegate void PointCloudReceivedHandler(Profile profile);

    /// <summary>
    /// Provides easy access to camera.
    /// </summary>
    public class CameraManager
    {
        /// <summary>
        /// The camera instance.
        /// </summary>
        private FsApi _camera;

        /// <summary>
        /// Settings of the sensor.
        /// </summary>
        private readonly ApplicationSettings _settings;

        /// <summary>
        /// Identifier for the camera.
        /// </summary>
        private string _cameraId;

        public string CameraId => _cameraId;

        /// <summary>
        /// Flag that is set to true, when the program is about to exist.
        /// </summary>
        private bool _exitRequested;

        /// <summary>
        /// The queue for the point clouds. 
        /// </summary>
        private readonly ConcurrentQueue<Profile> _queue = new ConcurrentQueue<Profile>();

        /// <summary>
        /// Gets or sets the camera version.
        /// </summary>
        public string CameraVersion { get; private set; }

        /// <summary>
        /// Gets or sets the serial number of camera.
        /// </summary>
        public string CameraSn { get; private set; }

        /// <summary>
        /// Gets the length of the local reception queue.
        /// </summary>
        public int QueueLength
        {
            get { return _queue.Count; }
        }

        /// <summary>
        /// Gets or sets the flag indicating whether the infinite queue size is enabled or not. If enabled, then queue size can cumulate over time.
        /// </summary>
        public bool IsInfiniteQueueSizeEnabled { get; set; }

        /// <summary>
        /// Gets the flag indicating whether the camera buffer (reception buffer inside FSAPI) is empty or not.
        /// </summary>
        public bool IsCameraBufferEmpty
        {
            get { return _header.ReceptionQueueSize == 0; }
        }

        /// <summary>
        /// Gets the flag indicating whether the HW supports Automatic Gain Control (AGC) or not.
        /// </summary>
        public bool IsAgcSupported { get; private set; }

        /// <summary>
        /// Gets the flag indicating whether Automatic Gain Control (AGC) is enabled or not.
        /// </summary>
        public bool IsAgcEnabled { get; private set; }

        /// <summary>
        /// LED pulse width [µs] adjusted by the Automatic Gain Control (AGC).
        /// </summary>
        private int _agcAdjustedLedPulseWidth;

        /// <summary>
        /// Processes received profiles, if needed.
        /// </summary>
        private readonly ProfileProcessor _profileProcessor;

        /// <summary>
        /// Last received profile header.
        /// </summary>
        private FsApi.Header _header;

        /// <summary>
        /// Gets the current LED pulse width in µs. If Automatic Gain Control (AGC) is in use, the value returned was adjusted by the AGC.
        /// </summary>
        public int CurrentLedPulseWidth
        {
            get
            {
                if (IsAgcSupported && IsAgcEnabled)
                {
                    return _agcAdjustedLedPulseWidth;
                }

                int pulseWidth;
                CameraStatusCode status = _camera.GetParameter(_cameraId, SensorParameter.LedDuration, out pulseWidth);
                if (status != CameraStatusCode.Ok)
                {
                    // Should never happen though.
                    throw new InvalidOperationException("Sensor connection issue.");
                }
                return pulseWidth;
            }
        }

        private void SelfLineCallback(
            int layerId,
            float[] zValues,
            float[] intensityValues,
            int lineLength,
            double xStep,
            FsApi.Header header)
        {
            if (_exitRequested)
            {
                return;
            }

            if (layerId > 0) return;

            _header = header;

            var processed = new Profile(zValues, intensityValues, lineLength, xStep, header);

            if(processed.Points == null)
                return;
            
            if (IsAgcSupported && IsAgcEnabled)
            {
                _agcAdjustedLedPulseWidth = (int)header.PulseWidth;
            }

            if (_grabbingProfile.Count <= 10)
                _grabbingProfile.Add(processed);
            else
                _grabbingEvent.Set();
        }

        readonly AutoResetEvent _grabbingEvent = new AutoResetEvent(true);

        readonly List<Profile> _grabbingProfile = new List<Profile>();
        public List<Profile> StartSelfGrabbing()
        {
            _camera.StopGrabbing(_cameraId);

            _camera.RemoveProfileCallback(_cameraId);

            _grabbingProfile.Clear();

            _camera.SetLineSortingOrder(_cameraId, SortingOrder.FromTopToBottom);

            _camera.SetLineCallback(_cameraId, 0, SelfLineCallback);
           
            _camera.StartGrabbing(_cameraId);

            _grabbingEvent.WaitOne(3000);

            _camera.StopGrabbing(_cameraId);

            return _grabbingProfile;
        }


        private ExportLayer _selectedLayer;

        public ExportLayer SelectedLayer
        {
            get
            {
                return this._selectedLayer;
            }

            set
            {
                this._selectedLayer = value;

                switch (this._selectedLayer)
                {
                    case ExportLayer.All:
                        _camera.StopGrabbing(_cameraId);

                        this._camera.RemoveLineCallback(this._cameraId);
                        _camera.SetProfileCallback(_cameraId, ProfileReceptionCallback);

                        _camera.StartGrabbing(_cameraId);
                        break;

                    case ExportLayer.Top:
                        _camera.StopGrabbing(_cameraId);
                        _camera.RemoveProfileCallback(_cameraId);

                        _camera.SetLineSortingOrder(_cameraId, SortingOrder.FromTopToBottom);

                        _camera.SetLineCallback(_cameraId, 0, this.LineCallback);
                        _camera.StartGrabbing(_cameraId);
                        break;

                    case ExportLayer.Bottom:
                        _camera.StopGrabbing(_cameraId);
                        _camera.RemoveProfileCallback(_cameraId);

                        _camera.SetLineSortingOrder(_cameraId, SortingOrder.FromBottomToTop);

                        _camera.SetLineCallback(_cameraId, 0, this.LineCallback);
                        _camera.StartGrabbing(_cameraId);
                        break;

                    case ExportLayer.Brightest:
                        _camera.StopGrabbing(_cameraId);
                        _camera.RemoveProfileCallback(_cameraId);

                        _camera.SetLineSortingOrder(_cameraId, SortingOrder.FromMaxIntensityToLower);

                        _camera.SetLineCallback(_cameraId, 0, this.LineCallback);
                        _camera.StartGrabbing(_cameraId);
                        break;

                    default: break;
                }
            }
        }

        /// <summary>
        /// Event fires when a point cloud is received from the camera.
        /// </summary>
        public event PointCloudReceivedHandler OnPointCloudReceivedEvent;

        /// <summary>
        /// Starts the reception thread.
        /// </summary>
        public CameraManager(ApplicationSettings settings)
        {
            _settings = settings;
            _profileProcessor = new ProfileProcessor();
            _cameraId = null;
            IsInfiniteQueueSizeEnabled = false;

            var queueThread = new Thread(QueueThread) { Priority = ThreadPriority.Highest };
            queueThread.Start();
        }

        /// <summary>
        /// Stop grabbing and close the camera instance.
        /// </summary>
        public void Close()
        {
            _exitRequested = true;

            if (string.IsNullOrEmpty(_cameraId))
            {
                return;
            }


            _camera.StopGrabbing(_cameraId);

            Thread.Sleep(500);

            _camera.Close();
        }

        private void LineCallback(
            int layerId,
            float[] zValues,
            float[] intensityValues,
            int lineLength,
            double xStep,
            FsApi.Header header)
        {
            
            if (_exitRequested)
            {
                return;
            }

            if (layerId > 0) return;

            _header = header;
            Profile processed = new Profile(zValues, intensityValues, lineLength, xStep, header);

            
            double distanceBetweenLines = 10000; 

            this._coverGlassCalculator.MeasureAngle(processed, distanceBetweenLines,true);

            _queue.Enqueue(processed);

            if (IsAgcSupported && IsAgcEnabled)
            {
                _agcAdjustedLedPulseWidth = (int)header.PulseWidth;
            }

            if (!IsInfiniteQueueSizeEnabled && QueueLength > Defines.QueueMaxLength)
            {
                Profile overflow;
                _queue.TryDequeue(out overflow);
            }
            
        }

        private readonly CoverGlassCalculator _coverGlassCalculator = new CoverGlassCalculator();

        /// <summary>
        /// Enqueues profiles into an application buffer.
        /// </summary>
        /// <param name="profile">Profile received from the FSAPI.</param>
        /// <param name="header">Header for the point cloud.</param>
        private void ProfileReceptionCallback(IList<FsApi.Point> profile, FsApi.Header header)
         {
            if (_exitRequested)
            {
                return;
            }

            _header = header;

            try
            {
                Profile raw = new Profile(profile, header);
                Profile processed;
                double pixelWidthUm = 1000 * _settings.AveragePixelWidth;

                switch (SelectedLayer)
                {
                    case ExportLayer.All:
                        processed = raw;
                        break;
                        /*
                    case ExportLayer.Top:
                    case ExportLayer.Bottom:
                    case ExportLayer.Brightest:
                        _profileProcessor.SortAndFilter(raw, 0.0, pixelWidthUm * Defines.SensorWidth, pixelWidthUm, SelectedLayer);
                        processed = new Profile(_profileProcessor.Layers[0].Points, _profileProcessor.Layers[0].PointCount, header);

                        // Other layers are available in arrays (layerId = 0-3)
                        //_profileProcessor.Layers[layerId].Points 
                        //_profileProcessor.Layers[layerId].PointCount
                        break;
                        */
                    default:
                        throw new ArgumentOutOfRangeException(string.Format("Unsupported layer {0}", SelectedLayer));
                }

                if (IsAgcEnabled)
                    // AGC uses the entire signal regardless of filtering we do in the application.
                    processed.AverageIntensity = raw.Points.Any() ? raw.Points.Average(p => p.Intensity) : 0.0f;
                else
                    processed.AverageIntensity = processed.Points.Any() ? processed.Points.Average(p => p.Intensity) : 0.0f;

                _queue.Enqueue(processed);

                if (IsAgcSupported && IsAgcEnabled)
                {
                    _agcAdjustedLedPulseWidth = (int)header.PulseWidth;
                }

                if (!IsInfiniteQueueSizeEnabled && QueueLength > Defines.QueueMaxLength)
                {
                    Profile overflow;
                    _queue.TryDequeue(out overflow);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        /// Initializes camera parameters.
        /// </summary>
        /// <returns>Operation outcome.</returns>
        public CameraStatusCode InitializeCamera()
        {
            int cameraCount;
            List<string> cameraIds;
			string cameraVersion, cameraSn;

            _header.ReceptionQueueSize = 0;

            _camera = new FsApi();
            
            var cameraStatus = _camera.Open(out cameraCount, out cameraIds, Defines.SensorDiscoveryTimeout);

            if (cameraStatus != CameraStatusCode.Ok)
                return cameraStatus;

            if (cameraCount == 0)
            {
                _camera.Close();
                return CameraStatusCode.NotConnected;
            }

            _cameraId = cameraIds.First();
            cameraStatus = _camera.Connect(_cameraId, _settings.IpAddress);
            if (cameraStatus != CameraStatusCode.Ok)
                return cameraStatus;
			
            cameraStatus = _camera.GetDeviceVersion(_cameraId, out cameraVersion);
            CameraVersion = cameraVersion;
            if (cameraStatus != CameraStatusCode.Ok)
                return cameraStatus;

            cameraStatus = _camera.GetSerialNumber(_cameraId, out cameraSn);
            CameraSn = cameraSn;
            if (cameraStatus != CameraStatusCode.Ok)
                return cameraStatus;

            cameraStatus = _camera.SetProfileCallback(_cameraId, ProfileReceptionCallback);
            if (cameraStatus != CameraStatusCode.Ok)
                return cameraStatus;

            cameraStatus = _camera.SetParameter(_cameraId, SensorParameter.ZCalibrationFile, _settings.ZCalibrationFile);
            if (cameraStatus != CameraStatusCode.Ok)
                return cameraStatus;

            cameraStatus = _camera.SetParameter(_cameraId, SensorParameter.PeakYUnit, Defines.PeakUnitMicrometer);
            if (cameraStatus != CameraStatusCode.Ok)
                return cameraStatus;

            cameraStatus = _camera.SetParameter(_cameraId, SensorParameter.XCalibrationFile, _settings.XCalibrationFile);
            if (cameraStatus != CameraStatusCode.Ok)
                return cameraStatus;

            cameraStatus = _camera.SetParameter(_cameraId, SensorParameter.PeakXUnit, Defines.PeakUnitMicrometer);
            if (cameraStatus != CameraStatusCode.Ok)
                return cameraStatus;

            cameraStatus = _camera.SetParameter(_cameraId, SensorParameter.PeakThreshold, 10);
            if (cameraStatus != CameraStatusCode.Ok)
                return cameraStatus;

            // Set MTU, if needed. By default, it's 1500.
            cameraStatus = _camera.SetParameter(_cameraId, SensorParameter.Mtu, 9014);
            if (cameraStatus != CameraStatusCode.Ok)
                return cameraStatus;

            // Set Interpacket Delay, if needed. By default, it's 20.
            cameraStatus = _camera.SetParameter(_cameraId, SensorParameter.Ifg, 20);
            if (cameraStatus != CameraStatusCode.Ok)
                return cameraStatus;

            cameraStatus = EnableAgc(_settings.IsAgcEnabled);
            switch (cameraStatus)
            {
                case CameraStatusCode.CameraErrorHwNotSupported:
                    IsAgcSupported = false;
                    break;
                case CameraStatusCode.Ok:
                    cameraStatus = SetAgcTargetIntensity(_settings.AgcTargetIntensity);
                    if (cameraStatus != CameraStatusCode.Ok)
                        return cameraStatus;
                    IsAgcSupported = true;
                    break;
                default:
                    return cameraStatus;
            }

            cameraStatus = _camera.StartGrabbing(_cameraId);

            return cameraStatus;
        }
        
        /// <summary>
        /// Starts profile data acquisition.
        /// </summary>
        /// <returns>True, if operation was successful.</returns>
        public bool StartGrabbing()
        {
            return _camera.StartGrabbing(_cameraId) == CameraStatusCode.Ok;
        }

        /// <summary>
        /// Stops profile data acquisition.
        /// </summary>
        /// <returns>True, if operation was successful.</returns>
        public bool StopGrabbing()
        {
            var status = _camera.StopGrabbing(_cameraId) == CameraStatusCode.Ok;
            Thread.Sleep(500);
            return status;
        }

        /// <summary>
        /// Queue thread. Check if there are data available in the queue. If there is, get the
        /// latest point cloud and call the event handlers.
        /// </summary>
        private void QueueThread()
        {
            // Loop until the program quits.
            while (!_exitRequested)
            {
                try
                {
                    Profile profile = null;

                    if (_queue.Any())
                    {
                        if (!_queue.TryDequeue(out profile))
                        {
                            Thread.Sleep(0);
                        }
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }

                    try
                    {
                        if (OnPointCloudReceivedEvent != null && profile != null)
                        {
                            OnPointCloudReceivedEvent(profile);
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }

        /// <summary>
        /// Sets frequency to the camera.
        /// </summary>
        /// <param name="freq">Sampling frequency [hz]. For external triggering, this will be the intended frequency, which is used to set up ROI.</param>
        /// <param name="isExternalPulsingEnabled">If true, 0 (value for external) will be written to sensor.</param>
        /// <returns>Status code indicating operation outcome.</returns>
        public CameraStatusCode SetFreq(int freq, bool isExternalPulsingEnabled)
        {
            StopGrabbing();

            int height = (int)(47.5 / 129 * (1e6 / freq - 1548 / 47.5));

            // Safety margin.
            height -= (int)(0.2 * height);

            if (height > Defines.MaxSensorHeight)
                height = Defines.MaxSensorHeight;

            int offset = (Defines.MaxSensorHeight - height) / 2;

            CameraStatusCode cameraStatus = _camera.SetParameter(_cameraId, SensorParameter.Height, height);
            if (cameraStatus != CameraStatusCode.Ok)
                return cameraStatus;

            cameraStatus = _camera.SetParameter(_cameraId, SensorParameter.OffsetY, offset);
            if (cameraStatus != CameraStatusCode.Ok)
                return cameraStatus;

            cameraStatus = _camera.SetParameter(_cameraId, SensorParameter.PulseFrequency, isExternalPulsingEnabled ? 0 : freq);
            StartGrabbing();
            return cameraStatus;
        }

        /// <summary>
        /// Sets LED pulse width. Utilizes Automatic Gain Control (AGC) if it is supported by the HW.
        /// </summary>
        /// <param name="us">LED pulse width in µs. If AGC is available, this is used as starting reference.</param>
        /// <param name="maxUs">If AGC is available, this is the max. pulse width AGC can adjust into. Otherwise this is null.</param>
        /// <exception cref="ArgumentException">Start LED pulse is higher than max. LED pulse.</exception>
        /// <returns>Status code indicating operation outcome.</returns>
        public CameraStatusCode SetPulseWidth(int us, int? maxUs)
        {
            CameraStatusCode status;

            StopGrabbing();

            if (maxUs.HasValue && IsAgcEnabled)
            {
                if (us > maxUs.Value)
                    throw new ArgumentException("LED pulse start reference must be smaller than LED pulse upper limit (max).");

                status = _camera.SetParameter(_cameraId, SensorParameter.AgcWiLimit, maxUs.Value);

                if (status != CameraStatusCode.Ok)
                {
                    StartGrabbing();
                    return status;
                }
            }

            status = _camera.SetParameter(_cameraId, SensorParameter.LedDuration, us);
            StartGrabbing();
            return status;
        }

        public CameraStatusCode SetPulseWidth(int us)
        {
            StopGrabbing();

            return _camera.SetParameter(_cameraId, SensorParameter.LedDuration, us); ;
        }

        /// <summary>
        /// Enables/disables AGC.
        /// </summary>
        /// <param name="isEnabled">If true, tries to enable AGC.</param>
        /// <returns>CameraStatusCode.Ok if AGC is supported and enabled.</returns>
        public CameraStatusCode EnableAgc(bool isEnabled)
        {
            StopGrabbing();

            CameraStatusCode status = _camera.SetParameter(_cameraId, SensorParameter.AgcEnabled, isEnabled ? 1 : 0);

            if (status == CameraStatusCode.Ok)
                IsAgcEnabled = isEnabled;

            StartGrabbing();

            return status;
        }

        /// <summary>
        /// Sets AGC target intensity in grayscale.
        /// </summary>
        /// <param name="targetIntensity">Target intensity [0.0-255.0]</param>
        /// <returns>Status code indicating operation outcome.</returns>
        public CameraStatusCode SetAgcTargetIntensity(float targetIntensity)
        {
            StopGrabbing();
            var status = _camera.SetParameter(_cameraId, SensorParameter.AgcTarget, targetIntensity);
            StartGrabbing();
            return status;
        }
    }
}