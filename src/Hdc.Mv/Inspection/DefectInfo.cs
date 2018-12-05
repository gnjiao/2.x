﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Hdc.Mv.Inspection
{
    [Serializable]
    public class DefectInfo
    {
        public int Index { get; set; }
        public int TypeCode { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Size { get; set; }
    }
}