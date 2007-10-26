using System;
using xeus2.Properties;
using xeus2.xeus.Utilities;
using System.Timers;

namespace xeus2.xeus.Core
{
    public enum Oldness
    {
        Recent,
        Older,
        Old,
        Oldest,
        Undefined
    }

    public class RelativeOldness : NotifyInfoDispatcher
    {
        static readonly Timer _timeTimer = new Timer(5000.0);

        public DateTime? _dateTime = null;

        static RelativeOldness()
        {
            _timeTimer.AutoReset = true;
            _timeTimer.Start();
        }

        public RelativeOldness()
        {
            _timeTimer.Elapsed += _timeTimer_Elapsed;
        }

        ~RelativeOldness()
        {
            _timeTimer.Elapsed -= _timeTimer_Elapsed;
        }

        void _timeTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            RefreshRelativeTime();
        }

        public RelativeOldness(DateTime dateTime) : this()
        {
            _dateTime = dateTime;
            
            RefreshRelativeTime();
        }

        private Oldness _oldness = Oldness.Undefined;
        private string _relativeTime = String.Empty;

        Oldness GetOldness()
        {
            if (_dateTime == null)
            {
                return Oldness.Undefined;
            }

            TimeSpan oldness = System.DateTime.Now - _dateTime.Value;

            if (oldness < new TimeSpan(0, Settings.Default.UI_MsgOldnest_Recent_Min, 0))
            {
                return Oldness.Recent;
            }
            else if (oldness < new TimeSpan(0, Settings.Default.UI_MsgOldnest_Older_Min, 0))
            {
                return Oldness.Older;
            }
            else if (oldness < new TimeSpan(0, Settings.Default.UI_MsgOldnest_Old_Min, 0))
            {
                return Oldness.Old;
            }
            else
            {
                return Oldness.Oldest;
            }
        }

        public Oldness Oldness
        {
            get
            {
                return _oldness;
            }
        }

        public DateTime? DateTime
        {
            get
            {
                return _dateTime;
            }
        }

        public string RelativeTime
        {
            get
            {
                return _relativeTime;
            }
        }

        void RefreshRelativeTime()
        {
            string relativeTime = TimeUtilities.FormatRelativeTime(_dateTime);
            Oldness oldness = GetOldness();

            if (relativeTime != _relativeTime)
            {
                _relativeTime = relativeTime;
                NotifyPropertyChanged("RelativeTime");
            }

            if (oldness != _oldness)
            {
                _oldness = oldness;
                NotifyPropertyChanged("Oldness");
            }
        }
    }
}