using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Config;
using Topshelf;

namespace Hdc.Mv.LocatingService
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x => //1
            {
                x.Service<Bootstrapper>(s => //2
                {
                    s.ConstructUsing(name => new Bootstrapper()); //4
                    s.WhenStarted(tc =>
                    {
                        XmlConfigurator.ConfigureAndWatch(new FileInfo(".\\log4net.config"));
                        tc.Start();
                    }); //5
                    s.WhenStopped(tc => tc.Stop()); //6
                });
                x.RunAsLocalSystem(); //7

                x.SetDescription("Hdc.Mv.LocatingService"); //8
                x.SetDisplayName("Hdc.Mv.LocatingService"); //9
                x.SetServiceName("Hdc.Mv.LocatingService"); //10
            });
        }
    }
}
