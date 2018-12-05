using System;
using System.Threading.Tasks;
using Vins.ML.Domain;

namespace Hdc.Measuring
{
    [Serializable]
    public class MqWorkpieceTagService : IWorkpieceTagService
    {
        private bool _isInitialized;

        public void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;

            MqInitializer.Bus.SubscribeAsync<TotalCountChangedMqEvent>(SubscriptionId, x => Task.Run(() => { Tag = x.TotalCount; }));

            var rep = MqInitializer.Bus.Request<QueryTotalCountRequest,
                QueryTotalCountResponse>(new QueryTotalCountRequest());

            Tag = rep.TotalCount;
        }

        public int Tag { get; private set; }

        public string SubscriptionId { get; set; }
    }
}