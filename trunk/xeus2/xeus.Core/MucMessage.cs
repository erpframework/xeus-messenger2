using System ;
using agsXMPP.protocol.client ;

namespace xeus2.xeus.Core
{
	internal class MucMessage
	{
		private readonly Message _message ;
		private readonly MucContact _sender ;

		private DateTime _dateTime = DateTime.Now ;

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

        public DateTime RelativeTime
        {
            get
            {
                return _dateTime;
            }
        }

		public override string ToString()
		{
			return string.Format( "({2}) {0}: {1}", Sender, Body, DateTime ) ;
		}
	}
}