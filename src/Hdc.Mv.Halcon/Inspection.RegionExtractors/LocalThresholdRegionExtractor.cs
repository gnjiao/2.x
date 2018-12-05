using System;
using System.Windows.Markup;
using HalconDotNet;
using Hdc.Diagnostics;
using Hdc.Mv.Halcon;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class LocalThresholdRegionExtractor : RegionExtractorBase, IRegionExtractor
    {
        protected override HRegion ExtractInner(HImage image)
        {
            var region = image.LocalThreshold(Method, LightDark.ToHalconString(), GenParamName, GenParamValue);
            return region;
        }

        public string Method { get; set; } = "adapted_std_deviation";
        public LightDark LightDark { get; set; } = LightDark.Dark;
        public string[] GenParamName { get; set; } = new string[] { };
        public double[] GenParamValue { get; set; } = new double[] { };


//      Write in Xamls like this:
//      e.g., set GenParamName to "mask_size" and GenParamValue to 15
//      <LocalThresholdRegionExtractor>
//      	<LocalThresholdRegionExtractor.GenParamName>
//      		<sys:String>mask_size</sys:String>
//      	</LocalThresholdRegionExtractor.GenParamName>
//      	<LocalThresholdRegionExtractor.GenParamValue>
//      		<sys:Double>15</sys:Double>
//      	</LocalThresholdRegionExtractor.GenParamValue>
//      </LocalThresholdRegionExtractor>

//      If u want to use custom params like above, remember to add namespace at the head of the xaml:
//      xmlns:sys="clr-namespace:System;assembly=mscorlib" 

    }
}