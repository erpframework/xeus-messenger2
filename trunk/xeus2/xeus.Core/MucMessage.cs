using agsXMPP.protocol.client ;

namespace xeus2.xeus.Core
{
	internal class MucMessage
	{
		private readonly Message _message ;
		private readonly MucContact _sender ;

		public MucMessage( Message message, MucContact sender )
		{
			_message = message ;
			_sender = sender ;
		}

		public string Body
		{
			get
			{
				return _message.Body ;
			}
		}

		public MucContact Sender
		{
			get
			{
				return _sender ;
			}
		}

		public override string ToString()
		{
			if ( Sender == null )
			{
				return string.Format( "{0}: {1}", _message.Nickname, Body ) ;
			}
			else
			{
				return string.Format( "{0}: {1}", Sender, Body ) ;
			}
		}
	}
}