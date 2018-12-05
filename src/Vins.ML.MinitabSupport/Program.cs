using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hdc.Measuring.Minitab;
using Hdc.Reflection;

namespace Vins.ML.MinitabSupport
{
    internal class Program
    {
        private string _assemblyDirectoryPath;

        private static void Main(string[] args)
        {
            new Program();
        }

        public Program()
        {
            _assemblyDirectoryPath = typeof (Program).Assembly.GetAssemblyDirectoryPath();


            var excelPath =
                @"""D:\Vins.ML\2016-06-20_30_From_ML\S100\2016-06-21_01_Merged\_2016-06-21_00.38.59\Report_2016-06-21_00.38.59_MOD.xlsx""";

            var config = ConfigProvider.Config;

            var cmd2 = MinitabOperations.GetCommand(config);

            OutputMtb(config.OutputMtbFileName, cmd2);
        }

        private void OutputMtb(string fileName, string command)
        {
            using (var fs = new FileStream("fileName", FileMode.Create))
            {
                using (var tw = new StreamWriter(fs))
                {
                    tw.Write(command);
                }
            }
        }
    }
}