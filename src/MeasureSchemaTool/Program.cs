using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hdc.Measuring;
using Hdc.Reflection;
using Hdc.Serialization;
using Vins.ML.MeasureStationService;

namespace MeasureSchemaTool
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var assemblyDirectoryPath = typeof (Program).Assembly.GetAssemblyDirectoryPath();
            var measureConfigsDir = Path.Combine(assemblyDirectoryPath, "_MeasureConfigs");

//            Console.WriteLine("Press any key to create MeasureSchemas in: " + measureConfigsDir);
//            Console.ReadKey();
//            Console.WriteLine();
            CreateMeasureSchemas(measureConfigsDir);
            Console.WriteLine("Create sucessful. at " + DateTime.Now);
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static void CreateMeasureSchemas(string dir)
        {
//            var simDir = Path.Combine(dir, MeasureDeviceType.Sim.ToString());
//            Directory.CreateDirectory(simDir);
//            var simMeasureSchema = MeasureSchemaFactory.GetSimMeasureSchema();
//            simMeasureSchema.SerializeToXamlFile(simDir + "\\MeasureSchema.xaml");

            var lkDir = Path.Combine(dir, MeasureDeviceType.LK.ToString());
            Directory.CreateDirectory(lkDir);
            var lkMeasureSchema = MeasureSchemaFactory.GetLKMeasureSchema();
            lkMeasureSchema.SerializeToXamlFile(lkDir + "\\MeasureSchema.xaml");

//            var ljvDir = Path.Combine(dir, MeasureDeviceType.LJV.ToString());
//            Directory.CreateDirectory(ljvDir);
//            var lgvMeasureSchema = MeasureSchemaFactory.GetLgvMeasureSchema();
//            lgvMeasureSchema.SerializeToXamlFile(ljvDir + "\\MeasureSchema.xaml");
        }
    }
}