using System.Threading.Tasks;
using Hdc.Measuring;

namespace Vins.ML.Infrastructure
{
    public static class MeasureResultRepositoryEx
    {
        public static Task<StationResult> StoreMeasureResultAsync(this IMeasureResultRepository repository,
            StationResult stationResult)
        {
            return Task.Run(() => repository.StoreStationResult(stationResult));
        }

        public static Task<WorkpieceResult> QueryWorkpieceResultByWorkpieceTagAsync(
            this IMeasureResultRepository repository, int workpieceTag)
        {
            return Task.Run(() => repository.QueryWorkpieceResultByWorkpieceTag(workpieceTag));
        }

        public static Task<WorkpieceResult> QueryLastWorkpieceResultAsync(
            this IMeasureResultRepository repository)
        {
            return Task.Run(() => repository.QueryLastWorkpieceResult());
        }

        public static Task<WorkpieceResult> QueryFirstWorkpieceResultAsync(
            this IMeasureResultRepository repository)
        {
            return Task.Run(() => repository.QueryFirstWorkpieceResult());
        }

        public static Task<int> QueryTotalCountOfWorkpieceResultsAsync(
            this IMeasureResultRepository repository)
        {
            return Task.Run(() => repository.QueryTotalCountOfWorkpieceResults());
        }
    }
}