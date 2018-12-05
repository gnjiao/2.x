using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Hdc.Diagnostics;
using Hdc.Generators;
using Hdc.Measuring;
using Microsoft.Practices.Unity;
using Sqo;

namespace Vins.ML.Infrastructure
{
    public class SiaqodbRepository : IMeasureResultRepository
    {
        private string _dbFileName;
        private readonly ISequenceGenerator _generator = new SequentialIdentityGenerator();

        public string DbFileName
        {
            get { return _dbFileName; }
            set { _dbFileName = value; }
        }

        [InjectionConstructor]
        public SiaqodbRepository()
        {
            InitDb();
        }

        public SiaqodbRepository(string dbFileName)
        {
            InitDb();
            _dbFileName = dbFileName;
        }

        void InitDb()
        {
            SiaqodbConfigurator.SetLicense(
                @" re2/RDP7L2bdDQq0o604FAcNFrLFGMiR+AV4S7VVGUF1Z/n4iSnvm6IRReFdpwekvfQq61bDfKVHvSPSUz+9xA==");

            SiaqodbConfigurator.AddIgnore(nameof(CalculateDefinition.CalculateOperation), typeof (CalculateDefinition));
            SiaqodbConfigurator.AddIgnore(nameof(StationResult.MeasureResults), typeof (StationResult));
        }

        public List<StationResult> QueryAllStationResults()
        {
            throw new System.NotImplementedException();
        }

        public List<StationResult> QueryStationResultsByStationIndex(int stationIndex)
        {
            throw new System.NotImplementedException();
        }

        public WorkpieceResult QueryWorkpieceResultByWorkpieceTag(int workpieceTag)
        {
            using (var db = GetSiaqodb())
            {
                return QueryWorkpieceResultByWorkpieceTag_Inner(workpieceTag, db);
            }
        }

        private WorkpieceResult QueryWorkpieceResultByWorkpieceTag_Inner(int workpieceTag, Siaqodb db)
        {
            var sw = new NotifyStopwatch($"{nameof(QueryWorkpieceResultByWorkpieceTag)}, elapsed, at " + DateTime.Now);
            var query = db.Query<StationResult>();
            var stationResults = query
                .Where(x => x.WorkpieceTag == workpieceTag);

            var workpieceResults = stationResults.MergeWorkpieceResults();
            var workpieceResult = workpieceResults.FirstOrDefault();

            sw.Dispose();
            return workpieceResult;
        }

        public List<WorkpieceResult> QueryWorkpieceResultsByTake(int startTag, int count)
        {
            using (var db = GetSiaqodb())
            {
                var sw = new NotifyStopwatch($"{nameof(QueryWorkpieceResultsByTake)}, startTag={startTag}, count={count}, elapsed, at " + DateTime.Now);
                var query = db.Query<StationResult>();

                if (!query.Any())
                    return new List<WorkpieceResult>();

                var stationResults = query
                    .Where(x => (x.WorkpieceTag > startTag - 1) && (x.WorkpieceTag < startTag + count));

                var workpieceResults = stationResults.MergeWorkpieceResults();

                sw.Dispose();
                return workpieceResults;
            }
        }

        public List<WorkpieceResult> QueryWorkpieceResultsByTakeLast(int startTag, int count)
        {
            using (var db = GetSiaqodb())
            {
                var sw = new NotifyStopwatch($"{nameof(QueryWorkpieceResultsByTakeLast)}, startTag={startTag}, count={count}, elapsed, at " + DateTime.Now);
                var query = db.Query<StationResult>();

                if (!query.Any())
                    return new List<WorkpieceResult>();

                var actualStartTag = startTag <= 0 ? query.Max(x => x.WorkpieceTag) : startTag;

                var stationResults = query
                    .Where(x => (x.WorkpieceTag < actualStartTag + 1) && (x.WorkpieceTag > actualStartTag - count));

                var workpieceResults = stationResults.MergeWorkpieceResults();

                sw.Dispose();
                return workpieceResults;
            }
        }

        public List<WorkpieceResult> QueryAllWorkpieceResults()
        {
            using (var db = GetSiaqodb())
            {
                var sw = new NotifyStopwatch($"{nameof(QueryAllWorkpieceResults)}, elapsed, at " + DateTime.Now);
                var query = db.Query<StationResult>();

                if (!query.Any())
                    return new List<WorkpieceResult>();

                var stationResults = query.ToList();

                var workpieceResults = stationResults.MergeWorkpieceResults();
                return workpieceResults;
            }
        }

        public int QueryTotalCountOfWorkpieceResults()
        {
            using (var db = GetSiaqodb())
            {
                var wpcTags = db.Query<StationResult>().Select(x=>x.WorkpieceTag);
                var wpcCount = wpcTags.Distinct().Count();

                return wpcCount;
            }
        }

        public WorkpieceResult QueryLastWorkpieceResult()
        {
            using (var db = GetSiaqodb())
            {
                var wpcTags = db.Query<StationResult>().Select(x => x.WorkpieceTag);
                var maxwpcTag = wpcTags.Distinct().Max();

                var wpcResult = QueryWorkpieceResultByWorkpieceTag_Inner(maxwpcTag, db);
                return wpcResult;
            }
        }

        public WorkpieceResult QueryFirstWorkpieceResult()
        {
            using (var db = GetSiaqodb())
            {
                var wpcTags = db.Query<StationResult>().Select(x => x.WorkpieceTag);
                var maxwpcTag = wpcTags.Distinct().Min();

                var wpcResult = QueryWorkpieceResultByWorkpieceTag_Inner(maxwpcTag, db);
                return wpcResult;
            }
        }

        private int storeCounter;
        private object locker = new object();

        public StationResult StoreStationResult(StationResult stationResult)
        {
            lock (locker)
            {
//                var sw = new NotifyStopwatch($"{nameof(StoreStationResult)}, elapsed, at " + DateTime.Now);
                StationResult storeStationResult;

                using (var siaqodb = GetSiaqodb())
                {
//                    var count = siaqodb.Count<StationResult>();
                    //                        Console.WriteLine($"count={count}");

                    if (storeCounter%20 == 0)
                    {
                        //var sw = new NotifyStopwatch("storeCounter = " + storeCounter);

                        var currentCount = siaqodb.Count<StationResult>();
                        Console.WriteLine($"Repository.StationResult count = {currentCount}, at {DateTime.Now}");
                        Debug.WriteLine($"Repository.StationResult count = {currentCount}, at {DateTime.Now}");

                        if (MaxStationResultsCount > 0 && currentCount > MaxStationResultsCount)
                        {
                            var removeLength = currentCount - MaxStationResultsCount;
                            Console.WriteLine("removeLength = " + removeLength);
                            var removeResults = siaqodb.Query<StationResult>().Take(removeLength);

                            foreach (var result in removeResults)
                            {
                                siaqodb.Delete(result);
                            }
                        }

                        //sw.Dispose();
                    }

                    stationResult.Id = _generator.Next;
                    siaqodb.StoreObject(stationResult);
                    storeStationResult = stationResult;

//                    sw.Dispose();
                }

                storeCounter++;
                return storeStationResult;
            }
        }

        public int MaxStationResultsCount { get; set; }

        public void StoreStationResults(IEnumerable<StationResult> stationResults)
        {
            lock (locker)
            {
                Debug.WriteLine($"{nameof(StoreStationResults)}(): begin at " + DateTime.Now);

                var sw = new NotifyStopwatch($"{nameof(StoreStationResults)}");

                using (var siaqodb = GetSiaqodb())
                {
                    foreach (var stationResult in stationResults)
                    {
                        stationResult.Id = _generator.Next;
                       siaqodb.StoreObject(stationResult);
                    }
                }

                sw.Dispose();

                Debug.WriteLine($"{nameof(StoreStationResults)}(): end at " + DateTime.Now);

                storeCounter++;
            }
        }

        private Siaqodb GetSiaqodb()
        {
            return new Siaqodb(_dbFileName, 500*1024*1024);
//            return new Siaqodb(_dbFileName, Int32.MaxValue);
//            return new Siaqodb(_dbFileName);
//            return new Siaqodb(_dbFileName);
        }

        public int GetCountOfStationResults()
        {
            using (var siaqodb = new Siaqodb(_dbFileName))
            {
                var count = siaqodb.Count<StationResult>();
                Console.WriteLine($"{nameof(SiaqodbRepository)}.StationResult count = {count}, at {DateTime.Now}");
                Debug.WriteLine($"{nameof(SiaqodbRepository)}.StationResult count = {count}, at {DateTime.Now}");

                return count;
            }
        }
    }
}