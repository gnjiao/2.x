using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hdc;
using Hdc.Measuring;
using Hdc.Measuring.VinsML;
using Hdc.Reflection;
using Hdc.Serialization;

namespace Vins.ML.MeasureStationTest
{
    class Program
    {
        private readonly string _assemblyDirectoryPath;
        private readonly IMeasureService _measureService = new VinsMLMeasureService();

        static void Main(string[] args)
        {
            Task.Run(() => { new Program().Run(); });

            Process.GetCurrentProcess().WaitForExit();
        }

        public Program()
        {
            _assemblyDirectoryPath = typeof (Program).Assembly.GetAssemblyDirectoryPath();
        }

        private void Run()
        {
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.Opc.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.ADLink.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.VinsML.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.Epson.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.Keyence.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.Silicon.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.EasyNetQ.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Mv.ImageAcquisition.JAI.dll");

            var measureSchema = $"{_assemblyDirectoryPath}\\MeasureSchema.xaml"
                .DeserializeFromXamlFile<MeasureSchema>();

            var mockWorkpieceTagService = new MockWorkpieceTagService();
            measureSchema.WorkpieceTagService = mockWorkpieceTagService;

            _measureService.Initialize(measureSchema);
            _measureService.StationCompletedEvent.Subscribe(
                stationResult =>
                {
                    mockWorkpieceTagService.Tag++;
//                    AskWpcInPosition();
                });


            int maxCount = measureSchema.MeasureDefinitions.Count;

            //            AskWpcInPosition();

            Console.WriteLine("Press any key to exit");
        }

        private static void AskWpcInPosition()
        {
            Console.WriteLine("Press W for WorkpieceInPosition, Press S for SensorInPosition");
            var key = Console.ReadKey();

            if (key.Key == ConsoleKey.W)
            {
                InMemoryEventControllerService.Singletone.Trigger(new MeasureEvent()
                {
                    Message = "WpcInPosition",
                });
            }
        }
    }
}