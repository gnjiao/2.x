using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hdc.Measuring;

namespace Vins.ML.Infrastructure.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().Run();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private void Run()
        {
            var repo = new SiaqodbRepository("E:\\_TestDb1\\");
            repo.MaxStationResultsCount = 100000;

            while (true)
            {
                Console.WriteLine("Press key, S:Store, C:Count, M:Store Many");
                var key = Console.ReadKey();
                Console.Write("\n");
                switch (key.Key)
                {
                    case ConsoleKey.S:
                        for (int i = 0; i < 100; i++)
                        {
                            var stationResult = SampleGenerator.CreateMeasureResult();
                            repo.StoreStationResult(stationResult);
                        }
                        break;
                    case ConsoleKey.C:
                        var count = repo.GetCountOfStationResults();
                        break;
                    case ConsoleKey.M:
                        var ss = new List<StationResult>();
                        for (int i = 0; i < 1000; i++)
                        {
                            var wrs = SampleGenerator.CreateWorkpieceResultWithTag(10);
//                            var ss = ;
                            ss.AddRange(wrs.StationResults);
                        }
                        repo.StoreStationResults(ss);
                        break;
                }
            }
        }
    }
}