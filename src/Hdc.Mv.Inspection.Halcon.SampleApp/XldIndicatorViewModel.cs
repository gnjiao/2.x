using HalconDotNet;
using Hdc.Mvvm;

namespace Hdc.Mv.Mvvm
{
    public class XldIndicatorViewModel: ViewModel
    {
        private HXLD _xld;

        public HXLD Xld
        {
            get { return _xld; }
            set
            {
                if (Equals(_xld, value)) return;
                _xld = value;
                RaisePropertyChanged(() => Xld);
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