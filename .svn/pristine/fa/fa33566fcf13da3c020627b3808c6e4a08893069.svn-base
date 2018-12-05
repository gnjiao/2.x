using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Markup;
using HalconDotNet;
using Hdc.Diagnostics;
using Hdc.Mv.ImageAcquisition;
using Hdc.Mv.ImageAcquisition.Halcon;

namespace Hdc.Measuring
{
    [Serializable]
    [ContentProperty("HalconCamerals")]
    public class MultipleHalconCameraInitializer : IInitializer
    {
        private static MultipleHalconCameraInitializer _singleton;

        //public static HalconCamera SingletonHalconCamera { get; set; }        
        public Collection<HalconCamera> HalconCamerals { get; set; } = new Collection<HalconCamera>();        
        public ConcurrentDictionary<int, HImage> Images { get; private set; } 
            = new ConcurrentDictionary<int, HImage>();       
        public static MultipleHalconCameraInitializer Singleton => _singleton;

        public MultipleHalconCameraInitializer()
        {
            if (_singleton == null) _singleton = this;
        }

        public void Initialize()
        {
            var sw1 = new NotifyStopwatch("MultipleHalconCameraInitializer.Initialize(): HalconCamera..Init()");
           
            Console.WriteLine($"_camera init(), begin, at " + DateTime.Now);

            var cameraInitOk = false;

            for (var index = 0; index < HalconCamerals.Count; index++)
            {
                var halconCamera = HalconCamerals[index];
                cameraInitOk = false;

                for (var i = 0; i < 3; i++)
                {
                    Console.WriteLine($"_camera{index} init(), index:{i + 1}/3, begin, at " + DateTime.Now);
                    try
                    {
                        var ret = halconCamera.Init();
                        if (ret)
                        {
                            cameraInitOk = true;
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"_camera{index} init(), index:{i + 1}/3, error:{ex.Message}, at " + DateTime.Now);
                    }
                    finally
                    {
                        Console.WriteLine($"_camera{index} init(), index:{i + 1}/3, end, at " + DateTime.Now);
                    }
                }

                if (!cameraInitOk)
                    break;
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
        }
    }
}