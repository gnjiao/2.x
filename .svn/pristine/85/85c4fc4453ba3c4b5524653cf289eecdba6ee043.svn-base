using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hdc.Collections.Generic;
using Hdc.Diagnostics;
using Hdc.Generators;
using Hdc.IO;
using Hdc.Measuring;
using Hdc.Patterns;
using Hdc.Serialization;
using Microsoft.Practices.Unity;
using NDatabase;
using NDatabase.Api.Query;

namespace Vins.ML.Infrastructure
{
    public class NDatabaseRepository : IMeasureResultRepository
    {
        private string _dbFileName;
        private ISequenceGenerator _generator = new SequentialIdentityGenerator();

        public string DbFileName
        {
            get { return _dbFileName; }
            set { _dbFileName = value; }
        }

        [InjectionConstructor]
        public NDatabaseRepository()
        {
        }

        public NDatabaseRepository(string dbFileName)
        {
            _dbFileName = dbFileName;
        }

        public WorkpieceResult QueryWorkpieceResultByWorkpieceTag(int workpieceTag)
        {
            using (var odb = OdbFactory.Open(_dbFileName))
            {
                var sw = new NotifyStopwatch("QueryWorkpieceResultByWorkpieceTag elapsed, at " + DateTime.Now);
                var query = odb.Query<StationResult>();
                query.Descend("WorkpieceTag").Constrain(workpieceTag).Equal();
                var measureResults = query.Execute<StationResult>(true, 0, 30);

                var wp = new WorkpieceResult() {Tag = workpieceTag};

                var groups = measureResults.GroupBy(x => x.StationIndex);
                foreach (var @group in groups)
                {
                    if (!group.Any())
                        continue;

                    var result = group.OrderByDescending(x => x.MeasureDateTime).First();
                    wp.StationResults.Add(result);
                }

/*                var storeDir = "d:\\WpcResultStore\\";

                if (!Directory.Exists(storeDir))
                    Directory.CreateDirectory(storeDir);

                var todayDir = storeDir.CombilePath(DateTime.Now.ToString("yyyy-MM-dd"));

                if (!Directory.Exists(todayDir))
                    Directory.CreateDirectory(todayDir);

                var fileName = todayDir.CombilePath(wp.Tag.ToString()) + ".xaml";

                wp.SerializeToXamlFile(fileName);*/

                sw.Dispose();
                return wp;
            }
        }

        public List<WorkpieceResult> QueryWorkpieceResultsByTake(int startTag, int count)
        {
            using (var odb = OdbFactory.Open(_dbFileName))
            {
                var query = odb.Query<StationResult>();
                //.OrderAscending();
                //                var eq = descend.Constrain(startTag).Equal();
                var sizeGe = query.Descend("WorkpieceTag").Constrain(startTag - 1).Greater();
                var sizeLt = query.Descend("WorkpieceTag").Constrain(startTag + count).Smaller();
                var andQuery = sizeGe.And(sizeLt);
                query.Constrain(andQuery);

                //               var andQuery = sizeGe.And(sizeLt);
                var stationResults = query.Execute<StationResult>(true);

                var stationResultsGroupByWorkpiece = stationResults.GroupBy(x => x.WorkpieceTag);

                var workpieceResults = new List<WorkpieceResult>();
                foreach (var group in stationResultsGroupByWorkpiece)
                {
                    if (!group.Any())
                        continue;

                    var tag = group.First().WorkpieceTag;
                    var wp = new WorkpieceResult() {Tag = tag,};

                    for (int i = 0; i < 12; i++)
                    {
                        var i1 = i;
                        var srs = group.Where(x => x.StationIndex == i1).ToList();
                        if (!srs.Any())
                        {
                            continue;
                            var sr = new StationResult()
                            {
                                StationIndex = i,
                            };
                            wp.StationResults.Add(sr);
                        }
                        else
                        {
                            var sr = srs.OrderByDescending(x => x.MeasureDateTime).First();
                            wp.StationResults.Add(sr);
                        }
                    }


                    workpieceResults.Add(wp);
                }

                return workpieceResults;
            }
        }

        public List<WorkpieceResult> QueryWorkpieceResultsByTakeLast(int startTag, int count)
        {
            throw new NotImplementedException();
        }

        public List<WorkpieceResult> QueryAllWorkpieceResults()
        {
            using (var odb = OdbFactory.Open(_dbFileName))
            {
                var query = odb.Query<StationResult>();
                query.Descend("WorkpieceTag").OrderDescending();
                var stationResults = query.Execute<StationResult>(true);

                var stationResultsGroupByWorkpiece = stationResults.GroupBy(x => x.WorkpieceTag);

                var workpieceResults = new List<WorkpieceResult>();
                foreach (var group in stationResultsGroupByWorkpiece)
                {
                    if (!group.Any())
                        continue;

                    var tag = group.First().WorkpieceTag;
                    var wp = new WorkpieceResult() {Tag = tag,};

                    for (int i = 0; i < 5; i++)
                    {
                        var i1 = i;
                        var srs = group.Where(x => x.StationIndex == i1).ToList();
                        if (!srs.Any())
                        {
                            var sr = new StationResult()
                            {
                                StationIndex = i,
                            };
                            wp.StationResults.Add(sr);
                        }
                        else
                        {
                            var sr = srs.OrderByDescending(x => x.MeasureDateTime).First();
                            wp.StationResults.Add(sr);
                        }
                    }

                    //
                    //                    var groups = group.GroupBy(x => x.StationIndex);
                    //                    foreach (var @group2 in groups)
                    //                    {
                    //                        if (!group2.Any())
                    //                            continue;
                    //
                    //                        var result = group2.OrderByDescending(x => x.MeasureDateTime).First();
                    //                        wp.StationResults.Add(result);
                    //                    }

                    //

                    //                    wp.StationResults.AddRange(group);

                    workpieceResults.Add(wp);
                }

                return workpieceResults;
            }
        }

        public int QueryTotalCountOfWorkpieceResults()
        {
            throw new NotImplementedException();
        }

        public WorkpieceResult QueryLastWorkpieceResult()
        {
            throw new NotImplementedException();
        }

        public WorkpieceResult QueryFirstWorkpieceResult()
        {
            throw new NotImplementedException();
        }

        private object locker = new object();

        public StationResult StoreStationResult(StationResult stationResult)
        {
            Debug.WriteLine("NDatabaseRepository.StoreStationResult(): begin at " + DateTime.Now);
            StationResult storeStationResult;

            lock (locker)
            {
                long count = 0;

                using (var odb = OdbFactory.Open(_dbFileName))
                {
                    var indexManagerFor = odb.IndexManagerFor<StationResult>();
                    if (!indexManagerFor.ExistIndex("nameIndex"))
                        indexManagerFor.AddIndexOn("nameIndex", new[] {"WorkpieceTag"});

                    if (storeCounter % 100 == 0)
                    {
                        var query = odb.Query<StationResult>();
                        count = query.Count();
                        Console.WriteLine($"NDatabaseRepository.StationResult count = {count}, at {DateTime.Now}");
                        Debug.WriteLine($"NDatabaseRepository.StationResult count = {count}, at {DateTime.Now}");
                    }

                    stationResult.Id = _generator.Next;
                    var oid = odb.Store(stationResult);
                    var mr2 = odb.GetObjectFromId(oid);
                    storeStationResult = (StationResult) mr2;
                }

                if (count > 10000)
                {
                    var sw = new NotifyStopwatch("odb backup" + DateTime.Now);
                    var destFileName = _dbFileName + ".bak";

                    if (File.Exists(_dbFileName))
                    {
                        File.Copy(_dbFileName, destFileName, true);
                        File.Delete(_dbFileName);
                    }

                    List<StationResult> srs2 = new List<StationResult>();

                    using (var odb = OdbFactory.Open(destFileName))
                    {
                        var query = odb.Query<StationResult>();
                        query.Descend("WorkpieceTag").OrderDescending();
                        var firstStationResults = query.Execute<StationResult>(true, 0, 200);

                        foreach (var firstStationResult in firstStationResults)
                        {
                            srs2.Add(firstStationResult);
                        }
                    }

                    using (var odb = OdbFactory.Open(_dbFileName))
                    {
                        var indexManagerFor = odb.IndexManagerFor<StationResult>();
                        if (!indexManagerFor.ExistIndex("nameIndex"))
                            indexManagerFor.AddIndexOn("nameIndex", new[] { "WorkpieceTag" });

                        foreach (var sr in srs2)
                        {
                            var oid = odb.Store(sr);
                        }
                    }

                    sw.Dispose();
                }
            }
            Debug.WriteLine("NDatabaseRepository.StoreStationResult(): end at " + DateTime.Now);

            storeCounter++;
            return storeStationResult;
        }

        public int MaxStationResultsCount { get; set; }

        private int storeCounter;

        public List<StationResult> QueryAllStationResults()
        {
            using (var odb = OdbFactory.Open(_dbFileName))
            {
                var query = odb.Query<StationResult>();
                var mages = query.Execute<StationResult>();
                return mages.ToList();
            }
        }

        public List<StationResult> QueryStationResultsByStationIndex(int stationIndex)
        {
            using (var odb = OdbFactory.Open(_dbFileName))
            {
                var query = odb.Query<StationResult>();
                query.Descend("StationIndex").Constrain(stationIndex).Equal();
                var mages = query.Execute<StationResult>();
                return mages.ToList();
            }
        }

        public List<StationResult> QueryMeasureResultByWorkpieceTag(int workpieceTag)
        {
            using (var odb = OdbFactory.Open(_dbFileName))
            {
                var query = odb.Query<StationResult>();
                query.Descend("WorkpieceTag").Constrain(workpieceTag).Equal();
                var mages = query.Execute<StationResult>();
                return mages.ToList();
            }
        }
    }
}