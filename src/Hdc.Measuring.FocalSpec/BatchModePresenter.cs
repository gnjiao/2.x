// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BatchModePresenter.cs" company="FocalSpec Oy">
//   FocalSpec Oy 2016-
// </copyright>
// <summary>
//   Batch mode presenter is used for controlling batch recording.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using FocalSpec.FsApiNet.Model;

namespace Hdc.Measuring
{
    /// <summary>
    /// A batch mode presenter.
    /// </summary>
    public class BatchModePresenter
    {
        // TODO/FOCALSPEC Do we need this?
        /// <summary>
        /// The synchronization lock.
        /// </summary>
        private static readonly object SyncLock = new object();

        /// <summary>
        /// Object used for controlling the camera.
        /// </summary>
        private readonly CameraManager _cameraManager;

        /// <summary>
        /// The record container. This is null, if container is not available.
        /// </summary>
        private RecordContainer _recordContainer;

        /// <summary>
        /// The batch recording configuration.
        /// </summary>
        private readonly BatchConfiguration _batchConfiguration = new BatchConfiguration();

        /// <summary>
        /// Gets or sets a value indicating whether recording for this container is stopped.
        /// </summary>
        private bool _isStopped;
        
        /// <summary>
        /// Runtime settings.
        /// </summary>
        private readonly ApplicationSettings _applicationSettings;
        
        public BatchModePresenter()
        {
            _applicationSettings = ApplicationSettings.LoadFromFile();

            if (!_applicationSettings.IsZCalibrationDataSet ||
                !_applicationSettings.IsXCalibrationDataSet)
                throw new ArgumentException("Calibration files are not set. Closing application...");

            _cameraManager = new CameraManager(_applicationSettings);
            var cameraStatus = _cameraManager.InitializeCamera();
            if (cameraStatus != CameraStatusCode.Ok)
                throw new ArgumentException("Could not initialize camera.");

            _cameraManager.OnPointCloudReceivedEvent += OnPointCloudReceived;

            //_cameraManager.SelectedLayer = ExportLayer.Top;
        }

        public void SetLayer(ExportLayer layer)
        {
            _cameraManager.SelectedLayer = layer;
        }

        /// <summary>
        /// Clears the recording.
        /// </summary>
        public void ClearRecording()
        {
            lock (SyncLock)
            {
                StopRecording();
                _recordContainer = null;
            }

            Session.ViewMode = ViewMode.RealTime;
            StopBatch();
        }

        /// <summary>
        /// Saves the points to a file.
        /// </summary>
        /// <param name="path">Full pathname of the file. </param>
        public void SavePoints(string path)
        {
            lock (SyncLock)
            {
                if (_recordContainer != null)
                {
                    ExportBatch.SaveToPointCloudFile(path, _recordContainer.GetProfiles(), _batchConfiguration.ScanStepLength);
                }
            }
        }

        /// <summary>
        /// Starts a recording. Initializes a container for saving, and updates the buttons on the view.
        /// </summary>
        public void StartRecording()
        {
            Session.ViewMode = ViewMode.Recording;
            StartBatch(_batchConfiguration);

            _isStopped = false;

            lock (SyncLock)
            {
                // ReSharper disable once RedundantArgumentDefaultValue
                _recordContainer = new RecordContainer(_batchConfiguration.BatchLength);
            }
        }

        /// <summary>
        /// Starts a new batch.
        /// </summary>
        /// <param name="configuration">Batch condiguration.</param>
        private void StartBatch(BatchConfiguration configuration)
        {
            Flush();
            // Pass the ***intended*** frequency set up in the live mode.
            _cameraManager.SetFreq(Session.RealTimeCameraSetting.Freq, configuration.TriggerFrequency == Defines.ExternalTriggering);
            // Make sure we get all the profiles.
            _cameraManager.IsInfiniteQueueSizeEnabled = true;
        }

        /// <summary>
        /// Stops the batch.
        /// </summary>
        private void StopBatch()
        {
            Flush();

            _cameraManager.SetFreq(Session.RealTimeCameraSetting.Freq, Session.RealTimeCameraSetting.IsExternalPulsingEnabled);
            // Make sure UI does not get stuck.
            _cameraManager.IsInfiniteQueueSizeEnabled = false;
        }

        /// <summary>
        /// Ensures camera buffer is empty.
        /// </summary>
        private void Flush()
        {
            const double timeout = 3000;
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (!_cameraManager.IsCameraBufferEmpty)
            {
                if (stopwatch.Elapsed.TotalMilliseconds > timeout)
                    break;
            }
        }        

        /// <summary>
        /// Point cloud has been received from the camera. If a recording is active, add the profile to record container.
        /// </summary>
        /// <param name="profile">The profile.</param>
        private void OnPointCloudReceived(Profile profile)
        {
            lock (SyncLock)
            {
                if (_recordContainer == null || _isStopped)
                {
                    return;
                }

                if (_recordContainer.IsCollected)
                {
                    return;
                }

                _recordContainer.AddProfile(profile);

                if (_recordContainer.IsCollected)
                {
                    StopRecording();
                }
            }
        }

        /// <summary>
        /// Finish the active recording. Update the view accordingly.
        /// </summary>
        private void StopRecording()
        {
            _isStopped = true;

            Session.ViewMode = ViewMode.Batch;
        }
    }
}