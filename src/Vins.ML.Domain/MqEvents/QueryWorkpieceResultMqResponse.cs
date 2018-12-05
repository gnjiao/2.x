using System;
using Hdc.Measuring;

namespace Vins.ML.Domain
{
    public class QueryWorkpieceResultMqResponse
    {
        public Guid ClientGuid { get; set; }
        public int WorkpieceTag { get; set; }
        public WorkpieceResult WorkpieceResult { get; set; }
    }
}