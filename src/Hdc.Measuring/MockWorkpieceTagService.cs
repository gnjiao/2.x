using System;

namespace Hdc.Measuring
{
    [Serializable]
    public class MockWorkpieceTagService : IWorkpieceTagService
    {
        public void Initialize()
        {
        }

        public int Tag { get; set; }
    }
}