using System;
using System.Collections.ObjectModel;
using System.Windows.Markup;
using HalconDotNet;

namespace Hdc.Measuring
{
    [Serializable]
    [ContentProperty("GetImageFromCacheEntries")]
    public class GetImageFromCacheHFramegrabberProvider :IHFramegrabberProvider
    {
        public double GrabAsyncMaxDelay { get; set; } = -1.0;

        public HImage GrabImage()
        {
            HImage currentImage = null;
            foreach (var getImageFromCacheEntry in GetImageFromCacheEntries)
            {
                var image = HalconCameraInitializer.Singleton.Images[getImageFromCacheEntry.Index];

                if (currentImage == null)
                    currentImage = image;
                else
                {
                    currentImage = currentImage.AddImage(image,1.0,0.0);
                }
            }

            return currentImage;
        }

        public void SetFramegrabberParam(string param, string value)
        {
            return;
        }

        public void SetFramegrabberParam(HTuple param, HTuple value)
        {
            return;
        }

        public Collection<GetImageFromCacheEntry>  GetImageFromCacheEntries { get; set; } = new Collection<GetImageFromCacheEntry>();
    }
}