﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Config;
using Topshelf;

namespace Vins.ML.MeasureLineDataService
{
    class Program
    {
        static void Main(string[] args)
        {
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

                x.SetDescription("Vins.ML.MeasureLineDataService"); //8
                x.SetDisplayName("Vins.ML.MeasureLineDataService"); //9
                x.SetServiceName("Vins.ML.MeasureLineDataService"); //10
                x.UseLog4Net();
            });
        }
    }
}