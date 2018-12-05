using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Hdc.Mvvm;
using Vins.ML.MeasureLineManager.Annotations;

namespace Vins.ML.MeasureLineManager
{
    public class StationStatusViewModel : INotifyPropertyChanged
    {
        private string _stationName;
        private int _stationIndex;
        private string _launcherWatchdogTimeoutDescription;
        private DateTime _launcherWatchdogDateTime;
        private string _stationWatchdogTimeoutDescription;
        private string _currentMeasureSchemaName;
        private string _currentConfigName;
        private DateTime _stationWatchdogDateTime;
        public int DisplayIndex => StationIndex + 1;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int StationIndex
        {
            get { return _stationIndex; }
            set
            {
                if (_stationIndex == value) return;
                _stationIndex = value;
                OnPropertyChanged();
                OnPropertyChanged("DisplayIndex");
            }
        }

        public string StationName
        {
            get { return _stationName; }
            set
            {
                if (_stationName == value) return;
                _stationName = value;
                OnPropertyChanged();
            }
        }

        public DateTime LauncherWatchdogDateTime
        {
            get { return _launcherWatchdogDateTime; }
            set
            {
                if (_launcherWatchdogDateTime == value) return;
                _launcherWatchdogDateTime = value;
                OnPropertyChanged();
            }
        }

        public string LauncherWatchdogTimeoutDescription
        {
            get { return _launcherWatchdogTimeoutDescription; }
            set
            {
                if (_launcherWatchdogTimeoutDescription == value) return;
                _launcherWatchdogTimeoutDescription = value;
                OnPropertyChanged();
            }
        }

        public DateTime StationWatchdogDateTime
        {
            get { return _stationWatchdogDateTime; }
            set
            {
                if (_stationWatchdogDateTime == value) return;
                _stationWatchdogDateTime = value;
                OnPropertyChanged();
            }
        }

        public string StationWatchdogTimeoutDescription
        {
            get { return _stationWatchdogTimeoutDescription; }
            set
            {
                if (_stationWatchdogTimeoutDescription == value) return;
                _stationWatchdogTimeoutDescription = value;
                OnPropertyChanged();
            }
        }

        public string CurrentMeasureSchemaName
        {
            get { return _currentMeasureSchemaName; }
            set
            {
                if (_currentMeasureSchemaName == value) return;
                _currentMeasureSchemaName = value;
                OnPropertyChanged();
            }
        }

        public string CurrentConfigName
        {
            get { return _currentConfigName; }
            set
            {
                if (_currentConfigName == value) return;
                _currentConfigName = value;
                OnPropertyChanged();
            }
        }
    }
}