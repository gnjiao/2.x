using System.IO;
using Hdc.IO;
using Hdc.Mv.LocatingService;
using Hdc.Reflection;
using Hdc.Serialization;

namespace Hdc.Mv.RobotVision.LocatingService
{
    public static class ConfigProvider
    {
        static ConfigProvider()
        {
            var dir = typeof(Bootstrapper).Assembly.GetAssemblyDirectoryPath();
            var configFileName = Path.Combine(dir, "Hdc.Mv.RobotVision.LocatingService.Config.xaml");

            if (!configFileName.IsFileExist())
            {
                var newConfig = new Config();

                newConfig.SerializeToXamlFile(configFileName);
            }

            Config = configFileName.DeserializeFromXamlFile<Config>();
        }

        public static Config Config { get; set; }
    }
}