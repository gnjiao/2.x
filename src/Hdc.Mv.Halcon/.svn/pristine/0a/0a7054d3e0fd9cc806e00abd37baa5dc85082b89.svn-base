using System;
using System.ComponentModel;
using System.Linq;
using HalconDotNet;
using Hdc.Mv.Inspection;

namespace Hdc.Mv.Halcon
{
    [Serializable]
    [Block("RakeEdgeFinding", BlockCatagory.Geometry)]
    public class RakeEdgeFindingBlock : Block
    {
        // General
        [InputPort]
        [Browsable(true)]
        [Category(BlockPropertyCategories.Roi)]
        [DefaultValue(0.0)]
        public double StartX { get; set; }

        [InputPort]
        [Browsable(true)]
        [Category(BlockPropertyCategories.Roi)]
        public double StartY { get; set; }

        [InputPort]
        [Browsable(true)]
        [Category(BlockPropertyCategories.Roi)]
        public double EndX { get; set; }

        [InputPort]
        [Browsable(true)]
        [Category(BlockPropertyCategories.Roi)]
        public double EndY { get; set; }

        [InputPort]
        [Browsable(true)]
        [Category(BlockPropertyCategories.Roi)]
        public double RoiHalfWidth { get; set; }

        //
        [InputPort]
        [Browsable(true)]
        [Category(BlockPropertyCategories.Input)]
        public int RegionsCount { get; set; }

        [InputPort]
        [Browsable(true)]
        [Category(BlockPropertyCategories.Input)]
        public int RegionHeight { get; set; }

        [InputPort]
        [Browsable(true)]
        [Category(BlockPropertyCategories.Input)]
        public int RegionWidth { get; set; }

        [InputPort]
        [Browsable(true)]
        [Category(BlockPropertyCategories.Input)]
        public double Sigma { get; set; }

        [InputPort]
        [Browsable(true)]
        [Category(BlockPropertyCategories.Input)]
        public double Threshold { get; set; }

        [InputPort]
        [Browsable(true)]
        [Category(BlockPropertyCategories.Input)]
//        [DefaultValue(Mv.SelectionMode.First)]
        public SelectionMode SelectionMode { get; set; }

        [InputPort]
        [Browsable(true)]
        [Category(BlockPropertyCategories.Input)]
        public Transition Transition { get; set; }

        [InputPort]
        [Browsable(true)]
        [Category(BlockPropertyCategories.Input)]
        public HImage Image { get; set; }

        [OutputPort]
        [Browsable(true)]
        [ReadOnly(true)]
        [Category(BlockPropertyCategories.Output)]
        public Line Line { get; set; }

        public override void Process()
        {
            Line searchLine = new Line(StartX, StartY, EndX, EndY);

            var lines = HDevelopExport.Singletone.RakeEdgeLine(Image,
               line: searchLine,
               regionsCount: RegionsCount,
               regionHeight: RegionHeight,
               regionWidth: RegionWidth,
               sigma: Sigma,
               threshold: Threshold,
               transition: Transition,
               selectionMode: SelectionMode);

            if (lines.Any())
                Line = lines.First();

            return;
        }
    }
}