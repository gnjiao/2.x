using System;
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace Hdc.Measuring
{
    [Serializable]
    [ContentProperty(nameof(AdLinkOutputEntries))]
    public class AD7230Initializer : IInitializer
    {
        public void Initialize()
        {
            Console.WriteLine($"{nameof(AD7230Initializer)}.Initialize() begin at " + DateTime.Now);

            Console.WriteLine($"{nameof(AD7230Service)}.Initialize() begin at " + DateTime.Now);
            AD7230Service.Singletone.Initialize();
            Console.WriteLine($"{nameof(AD7230Service)}.Initialize() end at " + DateTime.Now);

            if (AdLinkOutputEntries != null)
                foreach (var entry in AdLinkOutputEntries)
                {
                    AD7230Service.Singletone.WriteLine(entry.ChannelIndex, entry.Value);
                }

            Console.WriteLine($"{nameof(AD7230Initializer)}.Initialize() end at " + DateTime.Now);
        }

        public Collection<ADLinkOutputEntry> AdLinkOutputEntries { get; set; } = new Collection<ADLinkOutputEntry>();
    }
}