using System.Collections.Generic;
using System.Threading.Tasks;
using Hdc.Measuring;

namespace Vins.ML.Infrastructure
{
    public interface IMeasureResultRepository
    {
        List<StationResult> QueryAllStationResults();
        List<StationResult> QueryStationResultsByStationIndex(int stationIndex);
        WorkpieceResult QueryWorkpieceResultByWorkpieceTag(int workpieceTag);
        List<WorkpieceResult> QueryWorkpieceResultsByTake(int startTag, int count);
        List<WorkpieceResult> QueryWorkpieceResultsByTakeLast(int startTag, int count);
        List<WorkpieceResult> QueryAllWorkpieceResults();
        int QueryTotalCountOfWorkpieceResults();
        WorkpieceResult QueryLastWorkpieceResult();
        WorkpieceResult QueryFirstWorkpieceResult();
        StationResult StoreStationResult(StationResult stationResult);
        int MaxStationResultsCount { get; set; }
    }
}