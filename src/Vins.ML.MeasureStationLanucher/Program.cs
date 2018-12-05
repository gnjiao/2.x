using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Hdc.Reflection;
using log4net.Config;
using Topshelf;

namespace Vins.ML.MeasureStationLanucher
{
    class Program
    {
        static void Main(string[] args)
        {
            var _assemblyDirectoryPath = typeof(Program).Assembly.GetAssemblyDirectoryPath();
            Assembly.LoadFile(_assemblyDirectoryPath + "\\Vins.ML.Domain.dll");

            var TopShelf_ServiceName = ConfigProvider.Config.TopShelf_ServiceName;
            if (string.IsNullOrEmpty(TopShelf_ServiceName))
                TopShelf_ServiceName = "Vins.ML.MeasureStationLanucher";

            HostFactory.Run(x => //1
            {
                x.Service<Bootstrapper>(s => //2
                {
                    XmlConfigurator.ConfigureAndWatch(new FileInfo(".\\log4net.config"));

                    s.ConstructUsing(name => new Bootstrapper()); //4
                    s.WhenStarted(tc =>
                    {
                        tc.Start();
                    }); //5
                    s.WhenStopped(tc => tc.Stop()); //6
                });
                x.RunAsLocalSystem(); //7

                x.SetDescription($"{TopShelf_ServiceName}. It provide ability to lanuch {TopShelf_ServiceName}."); //8
                x.SetDisplayName(TopShelf_ServiceName); //9
                x.SetServiceName(TopShelf_ServiceName); //10
                x.UseLog4Net();
            });
        }
    }
}
