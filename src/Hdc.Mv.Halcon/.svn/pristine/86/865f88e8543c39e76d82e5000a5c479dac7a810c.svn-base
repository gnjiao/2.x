using System;
using HalconDotNet;
using Hdc.Mv.Halcon;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class GenRectangle1RegionFromExistSmallestRectangle1RegionProcessor : IRegionProcessor
    {
        public HRegion Process(HRegion region)
        {
            var smallest = region[1].GetSmallestRectangle1();
            var empty = new HRegion();
            //Direction是矩形的方向，EdgeDirection是选择在哪条边上做这个矩形
            //比如在做左侧矩形的时候，这个矩形可能是根据他的左侧边来做，也可能是根据他的右侧边来做，当然，此情况下不会考虑上边和下边
            //EdgeDirection默认选择Center，也就是不参与运算
            switch (Direction)
            {
                case Direction.Left:
                    if (EdgeDirection == Direction.Right)
                    {
                        var col1LR = smallest.Column1 + smallest.Width - Width + HorizontalOffset;
                        var row1LR = smallest.Row1 + VerticalOffset;
                        var col2LR = col1LR + Width;
                        var row2LR = row1LR + Height;
                        empty.GenRectangle1((double)row1LR, col1LR, row2LR, col2LR);
                        return empty;
                    }
                    else
                    {
                        var col1L = smallest.Column1 - Width + HorizontalOffset;
                        var row1L = smallest.Row1 + VerticalOffset;
                        var col2L = col1L + Width;
                        var row2L = row1L + Height;
                        empty.GenRectangle1((double)row1L, col1L, row2L, col2L);
                        return empty;
                    }
                case Direction.Right:
                    if (EdgeDirection == Direction.Left)
                    {
                        var col1RL = smallest.Column1 + HorizontalOffset;
                        var row1RL = smallest.Row1 + VerticalOffset;
                        var col2RL = col1RL + Width;
                        var row2RL = row1RL + Height;
                        empty.GenRectangle1((double)row1RL, col1RL, row2RL, col2RL);
                        return empty;

                    }
                    else
                    {
                        var col1R = smallest.Column1 + smallest.Width + HorizontalOffset;
                        var row1R = smallest.Row1 + VerticalOffset;
                        var col2R = col1R + Width;
                        var row2R = row1R + Height;
                        empty.GenRectangle1((double)row1R, col1R, row2R, col2R);
                        return empty;
                    }
                case Direction.Top:
                    if (EdgeDirection == Direction.Bottom)
                    {
                        var col1TB = smallest.Column1 + HorizontalOffset;
                        var row1TB = smallest.Row1 + smallest.Height - Height + VerticalOffset;
                        var col2TB = col1TB + Width;
                        var row2TB = row1TB + Height;
                        empty.GenRectangle1((double)row1TB, col1TB, row2TB, col2TB);
                        return empty;
                    }
                    else
                    {
                        var col1T = smallest.Column1 + HorizontalOffset;
                        var row1T = smallest.Row1 - Height + VerticalOffset;
                        var col2T = col1T + Width;
                        var row2T = row1T + Height;
                        empty.GenRectangle1((double)row1T, col1T, row2T, col2T);
                        return empty;
                    }
                case Direction.Bottom:
                    if (EdgeDirection == Direction.Top)
                    {
                        var col1BT = smallest.Column1 + HorizontalOffset;
                        var row1BT = smallest.Row1 + VerticalOffset;
                        var col2BT = col1BT + Width;
                        var row2BT = row1BT + Height;
                        empty.GenRectangle1((double)row1BT, col1BT, row2BT, col2BT);
                        return empty;
                    }
                    else
                    {
                        var col1B = smallest.Column1 + HorizontalOffset;
                        var row1B = smallest.Row1 + smallest.Height + VerticalOffset;
                        var col2B = col1B + Width;
                        var row2B = row1B + Height;
                        empty.GenRectangle1((double)row1B, col1B, row2B, col2B);
                        return empty;
                    }
                default:
                    empty.GenEmptyRegion();
                    return empty;
            }
        }

        public Direction Direction { get; set; }
        public Direction EdgeDirection { get; set; } = Direction.Center;
        public int Width { get; set; }
        public int Height { get; set; }
        public int HorizontalOffset { get; set; }
        public int VerticalOffset { get; set; }
    }
}