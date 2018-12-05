using System.IO;
using Hdc.IO;
using Hdc.Measuring.Minitab;
using Hdc.Reflection;
using Hdc.Serialization;

namespace Vins.ML.MinitabSupport
{
    public class ConfigProvider
    {
        private static readonly string _assemblyDirectoryPath;

        static ConfigProvider()
        {
            _assemblyDirectoryPath = typeof (ConfigProvider).Assembly.GetAssemblyDirectoryPath();


            // Config
            var configFileName = Path.Combine(_assemblyDirectoryPath, "Vins.ML.MinitabSupport.Config.xaml");
            if (!configFileName.IsFileExist())
            {
                var newConfig = new MinitabCommandConfig()
                {
                    WOPEN_FilePath = "b:\\sample.xlsx",
                    SHEET_Count = 2,
                    SHEET_VNAMES = 17,
                    SHEET_FIRST = 18,
                    SHEET_NROWS = 196,
                };
                newConfig.SerializeToXamlFile(configFileName);
            }

            var config = configFileName.DeserializeFromXamlFile<MinitabCommandConfig>();
            Config = config;
        }

        public static Hdc.Measuring.Minitab.MinitabCommandConfig Config { get; set; }
    }
}