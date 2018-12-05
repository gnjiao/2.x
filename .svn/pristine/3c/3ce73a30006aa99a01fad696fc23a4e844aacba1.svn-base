using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hdc.Mvvm;

namespace Vins.ML.MeasureLineManager
{
    public class PlcWorkDataCountViewModel : ViewModel
    {
        private int _jobCount;
        private int _totalCount;
        private int _isNgCount;
        private int _isOkCount;
        private double _okPrecent;

        public int JobCount
        {
            get { return _jobCount; }
            set
            {
                if (Equals(_jobCount, value)) return;
                _jobCount = value;
                RaisePropertyChanged(() => JobCount);
            }
        }

        public int TotalCount
        {
            get { return _totalCount; }
            set
            {
                if (Equals(_totalCount, value)) return;
                _totalCount = value;
                RaisePropertyChanged(() => TotalCount);
            }
        }

        public int IsNgCount
        {
            get { return _isNgCount; }
            set
            {
                if (Equals(_isNgCount, value)) return;
                _isNgCount = value;
                RaisePropertyChanged(() => IsNgCount);
            }
        }

        public int IsOkCount
        {
            get { return _isOkCount; }
            set
            {
                if (Equals(_isOkCount, value)) return;
                _isOkCount = value;
                RaisePropertyChanged(() => IsOkCount);
            }
        }

        public double OkPrecent
        {
            get { return _okPrecent; }
            set
            {
                if (Equals(_okPrecent, value)) return;
                _okPrecent = value;
                RaisePropertyChanged(() => OkPrecent);
            }
        }
    }
}
