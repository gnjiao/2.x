using System;
using System.ComponentModel;
using HalconDotNet;

namespace Hdc.Mv.Halcon
{
    [Block("SpokeCircleFinding", BlockCatagory.Geometry)]
    public class SpokeCircleFindingBlock : Block
    {
        [Category(BlockPropertyCategories.InputControl)]
        [InputPort]
        public double CenterX { get; set; } = 500;

        [Category(BlockPropertyCategories.InputControl)]
        [InputPort]
        public double CenterY { get; set; } = 500;

        [Category(BlockPropertyCategories.InputControl)]
        [InputPort]
        public double InnerRadius { get; set; } = 50;

        [Category(BlockPropertyCategories.InputControl)]
        [InputPort]
        public double OuterRadius { get; set; } = 100;

        [Category(BlockPropertyCategories.InputControl)]
        [InputPort]
        public int RegionsCount { get; set; } = 50;

        [Category(BlockPropertyCategories.InputControl)]
        [InputPort]
        public int RegionWidth { get; set; } = 50;

        [Category(BlockPropertyCategories.InputControl)]
        [InputPort]
        public double Sigma { get; set; } = 1;

        [Category(BlockPropertyCategories.InputControl)]
        [InputPort]
        public double Threshold { get; set; } = 20;

        [Category(BlockPropertyCategories.InputControl)]
        [InputPort]
        public SelectionMode SelectionMode { get; set; } = Mv.SelectionMode.First;

        [Category(BlockPropertyCategories.InputControl)]
        [InputPort]
        public Transition Transition { get; set; } = Transition.All;

        [Category(BlockPropertyCategories.InputControl)]
        [InputPort]
        public CircleDirect Direct { get; set; } = CircleDirect.Inner;

        [Category(BlockPropertyCategories.InputControl)]
        [InputPort]
        public string EllipseMode { get; set; } = "Circle";

        /// <summary>
        /// List of values: 'fhuber', 'fitzgibbon', 'focpoints', 'fphuber', 'fptukey', 'ftukey', 'geohuber', 'geometric', 'geotukey', 'voss'
        /// </summary>
        [Category(BlockPropertyCategories.InputControl)]
        [InputPort]
        public string EllipseAlgorithm { get; set; } = "fitzgibbon";

        [Category(BlockPropertyCategories.OutputObject)]
        [OutputPort]
        public Circle Circle { get; set; }

        [Category(BlockPropertyCategories.InputObject)]
        [InputPort]
        public HImage Image { get; set; }

        public override void Process()
        {
            Circle foundCircle;
            double roundless;

            if (Image == null)
            {
                Status = BlockStatus.Error;
                Exception = new BlockException("Image == null");
                return;
            }

            if (Image.Key == IntPtr.Zero)
            {
                Status = BlockStatus.Error;
                Exception = new BlockException("Image.Key == IntPtr.Zero");
                return;
            }

            try
            {
                var isOK = HDevelopExport.Singletone.ExtractCircle(
                    Image,
                    CenterX,
                    CenterY,
                    InnerRadius,
                    OuterRadius,
                    out foundCircle,
                    out roundless,
                    RegionsCount,
                    RegionWidth,
                    Sigma,
                    Threshold,
                    SelectionMode,
                    Transition,
                    Direct,
                    EllipseMode,
                    EllipseAlgorithm
                    );

                if (isOK)
                {
                    Circle = new Circle(
                        foundCircle.CenterX,
                        foundCircle.CenterY,
                        foundCircle.Radius);
                    Status = BlockStatus.Valid;
                    return;
                }
                else
                {
                    Status = BlockStatus.Error;
                    Exception = new BlockException("ExtractCircle error. ");
                    return;
                }
            }
            catch (Exception ex)
            {
                Status = BlockStatus.Error;
                Exception = new BlockException("ExtractCircle error. ", ex);
                return;
            }
        }
    }
}