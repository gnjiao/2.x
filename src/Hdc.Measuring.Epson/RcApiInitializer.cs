using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class RcApiInitializer : IInitializer
    {
        public void Initialize()
        {
            if (string.IsNullOrEmpty(ProjectFileName))
            {
                throw new InvalidOperationException("ProjectFileName IsNullOrEmpty");
            }

            if (ConnectionNumber == 0)
                ConnectionNumber = 1;
            RcApiService.Singletone.Initialize(ProjectFileName, ConnectionNumber);
        }

        public string ProjectFileName { get; set; }

        public int ConnectionNumber { get; set; } = 1;
    }
}