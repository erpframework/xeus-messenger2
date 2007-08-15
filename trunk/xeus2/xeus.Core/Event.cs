using System ;

namespace xeus2.xeus.Core
{
	internal abstract class Event : NotifyInfoDispatcher
	{
		public enum EventSeverity
		{
			Debug,
			Info,
			Error,
            Exception,
			Fatal
		}

		private string _message = null ;
		private readonly EventSeverity _eventSeverity ;
		private readonly DateTime _time = DateTime.Now ;

		public Event( string message, EventSeverity eventSeverity )
		{
			_message = message ;
			_eventSeverity = eventSeverity ;
		}

		virtual public string Message
		{
			get
			{
				return _message ;
			}
		}

		public EventSeverity Severity
		{
			get
			{
				return _eventSeverity ;
			}
		}

		public DateTime Time
		{
			get
			{
				return _time ;
			}
		}

        public string RelativeTime
        {
            get
            {
                return Utilities.TimeUtilities.FormatRelativeTime(_time);
            }
        }

        public void RefreshRelativeTime()
        {
            NotifyPropertyChanged("RelativeTime");
        }
        
        public override string ToString()
		{
			return string.Format( "{0}: {1}", Severity, Message ) ;
		}
	}
}