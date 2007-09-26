using agsXMPP.protocol.client ;

namespace xeus2.xeus.Core
{
    public class EventError : Event
	{
		private readonly Error _error = null ;

		public EventError( string message, Error error ) : base( message, EventSeverity.Error )
		{
			_error = error ;
		}

		public Error IqError
		{
			get
			{
				return _error ;
			}
		}
	}
}
