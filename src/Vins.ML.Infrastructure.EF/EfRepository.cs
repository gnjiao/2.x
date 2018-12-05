using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Hdc.Measuring;

namespace Vins.ML.Infrastructure
{
    public class EfRepository : IMeasureResultRepository
    {
        public List<StationResult> QueryAllStationResults()
        {
            throw new NotImplementedException();
        }

        public List<StationResult> QueryStationResultsByStationIndex(int stationIndex)
        {
            throw new NotImplementedException();
        }

        public WorkpieceResult QueryWorkpieceResultByWorkpieceTag(int workpieceTag)
        {
            throw new NotImplementedException();
        }

        public List<WorkpieceResult> QueryWorkpieceResultsByTake(int startTag, int count)
        {
            throw new NotImplementedException();
        }

        public List<WorkpieceResult> QueryWorkpieceResultsByTakeLast(int startTag, int count)
        {
            throw new NotImplementedException();
        }

        public List<WorkpieceResult> QueryAllWorkpieceResults()
        {
            throw new NotImplementedException();
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

        public StationResult StoreStationResult(StationResult stationResult)
        {
            throw new NotImplementedException();
        }

        public int MaxStationResultsCount { get; set; }
    }
}
