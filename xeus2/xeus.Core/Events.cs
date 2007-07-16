using System.Windows.Threading ;

namespace xeus2.xeus.Core
{
	internal class Events : ObservableCollectionDisp< Event >
	{
		public delegate void EventItemCallback( object sender, Event myEvent ) ;
		public event EventItemCallback OnEventRaised ;

		private static Events _instance = new Events() ;

		public static Events Instance
		{
			get
			{
				return _instance ;
			}
		}

		public void OnEvent( object sender, Event myEvent )
		{
            App.InvokeSafe(App._dispatcherPriority,
			                new EventItemCallback( OnEventInternal ), sender, myEvent ) ;
		}

		public void OnEventInternal( object sender, Event myEvent )
		{
			Add( myEvent ) ;

			if ( OnEventRaised != null )
			{
				OnEventRaised( sender, myEvent ) ;
			}
#if DEBUG
			if ( myEvent.Severity >= Event.EventSeverity.Fatal )
			{
				throw new XeusException( myEvent.Message ) ;
			}
#endif
		}
	}
}