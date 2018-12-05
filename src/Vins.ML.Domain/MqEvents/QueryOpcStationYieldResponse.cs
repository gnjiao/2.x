using System.Collections.ObjectModel;

namespace Vins.ML.Domain
{
    public struct Yield
    {
        public int StationIndex;

        public int OkCount;

        public int TotleCount;
    }
    public class QueryOpcStationYieldResponse
    {
        public Collection<Yield> StationYields { get; set; }
    }
}
