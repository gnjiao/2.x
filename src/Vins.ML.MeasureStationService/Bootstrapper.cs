using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using Hdc;
using Hdc.IO;
using Hdc.Measuring;
using Hdc.Measuring.VinsML;
using Hdc.Reflection;
using Hdc.Serialization;
using Hdc.Toolkit;
using MassTransit;
using Topshelf.Logging;
using Vins.ML.Domain;

namespace Vins.ML.MeasureStationService
{
    public class Bootstrapper
    {
        private readonly string _assemblyDirectoryPath;
        public Config Config => ConfigProvider.Config;
        private readonly IMeasureService _measureService = new VinsMLMeasureService();

        public Bootstrapper()
        {
            _assemblyDirectoryPath = typeof(Program).Assembly.GetAssemblyDirectoryPath();
        }

        public void Start()
        {
            TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper.Start() begin, at " + DateTime.Now);

            // delete OCL*.tmp files
            TopShelfLogAndConsole.WriteLineInDebug("delete OCL*.tmp files: _assemblyDirectoryPath: " + _assemblyDirectoryPath);

            var dirFiles = Directory.GetFiles(_assemblyDirectoryPath);
            var fileInfos = dirFiles.Select(x => new FileInfo(x)).ToList();
            var fileNames = fileInfos.Select(x => x.Name).ToList();
            var tempFiles = fileNames.Where(x => x.EndsWith(".tmp") && x.StartsWith("OCL")).ToList();

            foreach (var tempFile in tempFiles)
            {
                File.Delete(tempFile);
                TopShelfLogAndConsole.WriteLineInDebug("delete OCL*.tmp files File.Delete: " + tempFile);
            }

            // Load dlls
            TopShelfLogAndConsole.WriteLineInDebug("Assembly.LoadFile() begin, at " + DateTime.Now);
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Mv.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Mv.Halcon.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.Opc.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.ADLink.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.VinsML.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.Epson.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.Keyence.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.Silicon.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.EasyNetQ.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.Reporting.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.Keyence.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.GoDEX.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.Halcon.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Mv.ImageAcquisition.JAI.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Mv.ImageAcquisition.Halcon.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.OmronZW.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.EsponTcpSrv.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.QueueScheduling.dll");
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Hdc.Measuring.FocalSpec.dll");
                       
            TopShelfLogAndConsole.WriteLineInDebug("Assembly.LoadFile() end, at " + DateTime.Now);

            // Deserialize MeasureSchema
            TopShelfLogAndConsole.WriteLineInDebug("DeserializeFromXamlFile<MeasureSchema>() begin, at " + DateTime.Now);
            MeasureSchema measureSchema;
            try
            {
                measureSchema = $"{_assemblyDirectoryPath}\\MeasureSchema.xaml"
                    .DeserializeFromXamlFile<MeasureSchema>();
            }
            catch (Exception e)
            {
                TopShelfLogAndConsole.WriteLineInDebug("MeasureSchema.DeserializeFromXamlFile error, throw exception");

#if !DEBUG
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                Environment.Exit(255);
#endif

                throw;
            }
            TopShelfLogAndConsole.WriteLineInDebug("DeserializeFromXamlFile<MeasureSchema>() end, at " + DateTime.Now);

            //
            TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper.Start(): _measureService.Initialize(measureSchema) begin, at " + DateTime.Now);
            _measureService.Initialize(measureSchema);
            TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper.Start(): _measureService.Initialize(measureSchema) end, at " + DateTime.Now);

            TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper.Start() end, at " + DateTime.Now);
        }

        public void Stop()
        {
            TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper.Stop() begin, at " + DateTime.Now);
            TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper.Stop() do nothing, at " + DateTime.Now);
            TopShelfLogAndConsole.WriteLineInDebug("Bootstrapper.Stop() End, at " + DateTime.Now);
        }
    }
}