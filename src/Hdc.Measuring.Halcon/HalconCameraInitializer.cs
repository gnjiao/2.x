using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows.Markup;
using HalconDotNet;
using Hdc.Diagnostics;
using Hdc.Mv.ImageAcquisition;
using Hdc.Mv.ImageAcquisition.Halcon;

namespace Hdc.Measuring
{
    [Serializable]
    [ContentProperty(nameof(HalconCamera))]
    public class HalconCameraInitializer : IInitializer
    {
        private static HalconCameraInitializer _singleton;
        public HalconCamera HalconCamera { get; set; }

        public static HalconCamera SingletonHalconCamera { get; set; }

        public ConcurrentDictionary<int, HImage> Images { get; private set; } 
            = new ConcurrentDictionary<int, HImage>();

//        public List<HImage> Images2 { get; set; } = new List<HImage>();

        public static HalconCameraInitializer Singleton
        {
            get { return _singleton; }
        }

        public HalconCameraInitializer()
        {
            if (_singleton == null) _singleton = this;
        }

        public void Initialize()
        {
            var sw1 = new NotifyStopwatch("HalconCameraInitializer.Initialize(): HalconCamera..Init()");

            bool cameraInitOk = false;
            Console.WriteLine($"_camera init(), begin, at " + DateTime.Now);
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine($"_camera init(), index:{i + 1}/3, begin, at " + DateTime.Now);
                try
                {
                    var ret = HalconCamera.Init();
                    if (ret)
                    {
                        cameraInitOk = true;
                        break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine($"_camera init(), index:{i + 1}/3, error, at " + DateTime.Now);
                }
                finally
                {
                    Console.WriteLine($"_camera init(), index:{i + 1}/3, end, at " + DateTime.Now);
                }
            }

            if (cameraInitOk)
            {
                Console.WriteLine($"_camera init(), end successful, at " + DateTime.Now);
            }
            else
            {
                Console.WriteLine($"_camera init(), error, at " + DateTime.Now);
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                Environment.Exit(0);
            }

            sw1.Dispose();

            if (SingletonHalconCamera != null)
                throw new InvalidOperationException("HalconCameraInitializer.Initialize(): SingletonHalconCamera create twice");

            SingletonHalconCamera = HalconCamera;
        }
    }
}