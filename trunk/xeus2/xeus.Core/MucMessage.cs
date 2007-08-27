using System ;
using agsXMPP.protocol.client ;

namespace xeus2.xeus.Core
{
    public enum MucMessageOldness
    {
        Recent,
        Older,
        Old,
        Oldest
    }

	internal class MucMessage : NotifyInfoDispatcher
	{
		private readonly Message _message ;
		private readonly MucContact _sender ;

		private DateTime _dateTime = DateTime.Now ;

	    public MucMessageOldness MessageOldness
	    {
	        get
	        {
	            TimeSpan oldness = DateTime.Now - _dateTime;

                if (oldness < new TimeSpan(0, 1, 0))
                {
                    return MucMessageOldness.Recent;
                }
                else if (oldness < new TimeSpan(0, 5, 0))
                {
                    return MucMessageOldness.Older;
                }
                else if (oldness < new TimeSpan(0, 15, 0))
                {
                    return MucMessageOldness.Old;
                }
                else
                {
                    return MucMessageOldness.Oldest;
                }
	        }
	    }

		public MucMessage( Message message, MucContact sender )
		{
			_message = message ;
			_sender = sender ;

			if ( _message.XDelay != null )
			{
				_dateTime = _message.XDelay.Stamp ;
			}
		}

		public string Body
		{
			get
			{
				return _message.Body ;
			}
		}

		public string Subject
		{
			get
			{
				return _message.Subject ;
			}
		}

		public MucContact Contact
		{
			get
			{
				return _sender ;
			}
		}

		public string Sender
		{
			get
			{
				return _message.From.Resource ;
			}
		}

		public DateTime DateTime
		{
			get
			{
				return _dateTime ;
			}
		}

        public string RelativeTime
        {
            get
            {
                return Utilities.TimeUtilities.FormatRelativeTime(_dateTime);
            }
        }

        public void RefreshRelativeTime()
        {
            NotifyPropertyChanged("RelativeTime");
        }

		public override string ToString()
		{
			return string.Format( "({2}) {0}: {1}", Sender, Body, DateTime ) ;
		}
	}
}