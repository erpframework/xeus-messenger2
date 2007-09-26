using System;
using xeus2.Properties;
using xeus2.xeus.Utilities;

namespace xeus2.xeus.Core
{
    public enum MessageOldness
    {
        Recent,
        Older,
        Old,
        Oldest
    }

    public class MessageBase : NotifyInfoDispatcher
    {
        protected DateTime _dateTime = DateTime.Now;

        public MessageOldness MessageOldness
        {
            get
            {
                TimeSpan oldness = DateTime.Now - _dateTime;

                if (oldness < new TimeSpan(0, Settings.Default.UI_MsgOldnest_Recent_Min, 0))
                {
                    return MessageOldness.Recent;
                }
                else if (oldness < new TimeSpan(0, Settings.Default.UI_MsgOldnest_Older_Min, 0))
                {
                    return MessageOldness.Older;
                }
                else if (oldness < new TimeSpan(0, Settings.Default.UI_MsgOldnest_Old_Min, 0))
                {
                    return MessageOldness.Old;
                }
                else
                {
                    return MessageOldness.Oldest;
                }
            }
        }

        public DateTime DateTime
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
                return TimeUtilities.FormatRelativeTime(_dateTime);
            }
        }

        public void RefreshRelativeTime()
        {
            NotifyPropertyChanged("RelativeTime");
        }
    }
}