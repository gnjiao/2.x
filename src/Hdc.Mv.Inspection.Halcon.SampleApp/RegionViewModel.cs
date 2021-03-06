﻿using HalconDotNet;
using Hdc.Mvvm;

namespace Hdc.Mv.Inspection.Halcon.SampleApp
{
    public class RegionViewModel:ViewModel
    {

        private HRegion _region;

        public HRegion Region
        {
            get { return _region; }
            set
            {
                if (Equals(_region, value)) return;
                _region = value;
                RaisePropertyChanged(() => Region);
            }
        }

        private bool _displayEnabled = true;

        public bool DisplayEnabled
        {
            get { return _displayEnabled; }
            set
            {
                if (Equals(_displayEnabled, value)) return;
                _displayEnabled = value;
                RaisePropertyChanged(() => DisplayEnabled);
            }
        }
    }
}