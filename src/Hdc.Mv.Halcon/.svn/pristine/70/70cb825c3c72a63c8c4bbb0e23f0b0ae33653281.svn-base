using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Markup;
using HalconDotNet;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    //[ContentProperty("PointOfEdgeAndRadialLineDefinition")]
    public class EdgeSearchingOfRegionPointsDefinition : DefinitionBase
    {
        public string PointName { get; set; }

        //Below are Parameters of fit_line_contour_xld
        public string Algorithm { get; set; } = "tukey";
        /*Algorithm for the fitting of lines.
        Default value: 'tukey'
        List of values: 'drop', 'gauss', 'huber', 'regression', 'tukey'*/

        public int MaxNumPoints { get; set; } = -1;
        /*Maximum number of contour points used for the computation(-1 for all points).
        Default value: -1
        Restriction: MaxNumPoints >= 2*/

        public int ClippingEndPoints { get; set; } = 0;
        /*Number of points at the beginning and at the end of the contours to be ignored for the fitting.
        Default value: 0
        Restriction: ClippingEndPoints >= 0*/

        public int Iterations { get; set; } = 5;
        /*Maximum number of iterations(unused for 'regression').
        Default value: 5
        Restriction: Iterations >= 0*/

        public double ClippingFactor { get; set; } = 2.0;
        /*Clipping factor for the elimination of outliers(typical: 1.0 for 'huber' and 'drop' and 2.0 for 'tukey').
        Default value: 2.0
        List of values: 1.0, 1.5, 2.0, 2.5, 3.0
        Restriction: ClippingFactor > 0*/
    }
}