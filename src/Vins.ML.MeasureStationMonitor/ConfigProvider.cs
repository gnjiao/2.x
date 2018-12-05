﻿using System.IO;
using Hdc.IO;
using Hdc.Reflection;
using Hdc.Serialization;

namespace Vins.ML.MeasureStationMonitor
{
    public class ConfigProvider
    {
        private static readonly string _assemblyDirectoryPath;

        static ConfigProvider()
        {
            _assemblyDirectoryPath = typeof (ConfigProvider).Assembly.GetAssemblyDirectoryPath();


            // Config
            var configFileName = Path.Combine(_assemblyDirectoryPath, _assemblyDirectoryPath + "\\Vins.ML.MeasureStationMonitor.Config.xaml");
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