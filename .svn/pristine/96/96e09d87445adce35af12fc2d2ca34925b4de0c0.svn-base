using System.IO;
using Hdc.IO;
using Hdc.Reflection;
using Hdc.Serialization;
using Vins.ML.OpcService;

namespace MqTestClient
{
    public class ConfigProvider
    {
        private static readonly string _assemblyDirectoryPath;

        static ConfigProvider()
        {
            _assemblyDirectoryPath = typeof (ConfigProvider).Assembly.GetAssemblyDirectoryPath();


            // Config
            var configFileName = Path.Combine(_assemblyDirectoryPath, "MqTestClient.Config.xaml");
            if (!configFileName.IsFileExist())
            {
                var newConfig = new Config();
                newConfig.SerializeToXamlFile(configFileName);
            }

            var config = configFileName.DeserializeFromXamlFile<Config>();
            Config = config;
        }

        public static Config Config { get; set; }
    }
}