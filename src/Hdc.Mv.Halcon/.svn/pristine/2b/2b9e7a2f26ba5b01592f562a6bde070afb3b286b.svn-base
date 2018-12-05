using System;
using HalconDotNet;
using Hdc.Mv.Halcon;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class FitCircleContourXldProcessor : IXldProcessor
    {
        public HXLD Process(HXLD xld)
        {
            var cont = xld as HXLDCont;
            if (cont == null)
                return xld;

            var circle = cont.FitCircleContourXld();

            if (circle.Radius < 0.000001)
                return xld;

            var circleXld = new HXLDCont();
            circleXld.GenCircleContourXld(circle.Row, circle.Column, circle.Radius,
                circle.StartPhi, circle.EndPhi, circle.PointOrder, Resolution);

            return circleXld;
        }

        public double Resolution { get; set; } = 1;
    }
}