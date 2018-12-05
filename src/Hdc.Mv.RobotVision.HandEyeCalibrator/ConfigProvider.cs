using System.IO;
using System.Windows;
using Hdc.IO;
using Hdc.Reflection;
using Hdc.Serialization;

namespace Hdc.Mv.RobotVision.HandEyeCalibrator
{
    public static class ConfigProvider
    {
        static ConfigProvider()
        {
            var dir = typeof(ConfigProvider).Assembly.GetAssemblyDirectoryPath();
            var configFileName = Path.Combine(dir, "Hdc.Mv.RobotVision.HandEyeCalibrator.Config.xaml");

            if (!configFileName.IsFileExist())
            {
                var newConfig = new Config();
//                for (int i = 0; i < 9; i++)
//                {
//                    newConfig.CalibPlate_CameraVectors.Add(new Vector(i,i));
//                    newConfig.CalibPlate_WorldVectors.Add(new Vector(i,i));
//                }
//                newConfig.CalibPlate_ToolInBaseVector = new Vector();
//                newConfig.Source_OriInCamVector = new Vector();
//                newConfig.Source_RefInCamVector = new Vector();
//                newConfig.Source_OriToolInBaseVector = new Vector();
//                newConfig.Source_RefToolInBaseVector = new Vector();

                newConfig.SerializeToXamlFile(configFileName);
            }

            Config = configFileName.DeserializeFromXamlFile<Config>();
        }

        public static Config Config { get; set; }
    }
}