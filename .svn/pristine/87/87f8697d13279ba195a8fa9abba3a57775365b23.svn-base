using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FocalSpec.FsApiNet.Model;
using Hdc.Diagnostics;

namespace Hdc.Measuring
{
    [Serializable]
    public class FocalSpecInspectorInitializer : IInitializer
    {
        const int MAXNUMPTS = 10000;
        public static FocalSpecInspectorInitializer Singleton { get; private set; }        
        public ApplicationSettings FocalSpecApplicationSettings { get; set; }
        public BatchConfiguration FocalSpecBatchConfiguration { get; set; }
        public int MaxBatchLength { get; set; }
        public bool CameraInitOk { get; set; }
                
        private bool _isStopped;
        private CameraManager _cameraManager;
        private RecordContainer _recordContainer;
        private static readonly object SyncLock = new object();
        public CameraManager CameraManager => _cameraManager;

        public static List<double> ShortstandardConfigX = new List<double>();
        public static List<double> ShortstandardConfigY = new List<double>();
        public static List<double> LongstandardConfigX = new List<double>();
        public static List<double> LongstandardConfigY = new List<double>();

        public int nPtsRefShort = 0;
        public double[] xRefShort = new double[MAXNUMPTS];
        public double[] yRefShort = new double[MAXNUMPTS];

        public int nPtsRefLong = 0;
        public double[] xRefLong = new double[MAXNUMPTS];
        public double[] yRefLong = new double[MAXNUMPTS];

        public string ShortStandardConfigPath { get; set; } = "v300l_back_glass_assy1_short.csv";
        public string LongStandardConfigPath { get; set; } = "v300l_back_glass_assy2_long.csv";

        public FocalSpecInspectorInitializer()
        {
            if (Singleton != null)
                return;

            Singleton = this;
        }

        public void Initialize()
        {
            var sw1 = new NotifyStopwatch("FocalSpecInspectorInitializer.Initialize(): FocalSpecCameraManager..Init()");

            CameraInitOk = false;

            try
            {
                Console.WriteLine($"FocalSpecCameraManager init(), begin, at " + DateTime.Now);

                if (!FocalSpecApplicationSettings.IsZCalibrationDataSet ||
                    !FocalSpecApplicationSettings.IsXCalibrationDataSet)
                    throw new ArgumentException("Calibration files are not set. Closing application...");

                _cameraManager = new CameraManager(FocalSpecApplicationSettings);
                _recordContainer= new RecordContainer(MaxBatchLength);

                var cameraStatus = _cameraManager.InitializeCamera();
                if (cameraStatus != CameraStatusCode.Ok)
                {
                    _cameraManager.Close();
                    throw new ArgumentException("Could not initialize camera.");
                }

                //_cameraManager.OnPointCloudReceivedEvent += OnPointCloudReceived;

                CameraManager.StartSelfGrabbing();

                CameraInitOk = true;



                //mwLibInitialize(ShortStandardConfigPath, ShortstandardConfigX, ShortstandardConfigY);
                //mwLibInitialize(LongStandardConfigPath, LongstandardConfigX, LongstandardConfigY);

                NativeMethods.mwLibInit();
                                
                if (!NativeMethods.mwReadRefcsv(ShortStandardConfigPath,ref nPtsRefShort,xRefShort,yRefShort,.001))
                {
                    return;
                }

                if (!NativeMethods.mwReadRefcsv(LongStandardConfigPath, ref nPtsRefLong, xRefLong, yRefLong, .001))
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FocalSpecCameraManager init(),  error at {ex.Message}" + DateTime.Now);
            }

            if (CameraInitOk)
            {
                Console.WriteLine($"FocalSpecCameraManager init(), end successful, at " + DateTime.Now);
            }
            else
            {
                Console.WriteLine($"FocalSpecCameraManager init(), error, at " + DateTime.Now);
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                Environment.Exit(0);
            }
            
            sw1.Dispose();
        }

        private void mwLibInitialize(string standardConfigDataPath, List<double> xList, List<double> yList)
        {
            FileStream fileStream = null;
            StreamReader streamReader = null;
            try
            {
                fileStream = File.Open(standardConfigDataPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                streamReader = new StreamReader(fileStream, Encoding.UTF8);
                while (!streamReader.EndOfStream)
                {
                    var str = streamReader.ReadLine();
                    if (str != null)
                    {
                        var xu = str.Split(',');
                        var xGet = xu[0];
                        var yGet = xu[1];

                        xList.Add(Convert.ToDouble(xGet));
                        yList.Add(Convert.ToDouble(yGet));
                    }
                }
            }
            finally
            {
                streamReader?.Close();
                fileStream?.Close();
            }
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
                    ExportBatch.SaveToPointCloudFile(path, _recordContainer.GetProfiles(), FocalSpecBatchConfiguration.ScanStepLength);
                }
            }
        }

        /// <summary>
        /// Starts a recording. Initializes a container for saving, and updates the buttons on the view.
        /// </summary>
        public void StartRecording()
        {
            Session.ViewMode = ViewMode.Recording;
            StartBatch(FocalSpecBatchConfiguration);

            _isStopped = false;

            lock (SyncLock)
            {
                // ReSharper disable once RedundantArgumentDefaultValue
                _recordContainer = new RecordContainer(FocalSpecBatchConfiguration.BatchLength);
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
