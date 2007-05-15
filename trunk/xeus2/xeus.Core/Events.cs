using System.Windows.Threading ;

namespace xeus2.xeus.Core
{
	internal class Events : ObservableCollectionDisp< Event >
	{
		private delegate void EventItemCallback( Event discoInfo ) ;

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
			App.InvokeSafe( DispatcherPriority.Background,
			                new EventItemCallback( OnEvent ), myEvent ) ;
		}

		public void OnEvent( Event myEvent )
		{
			Add( myEvent ) ;
#if DEBUG
			if ( myEvent.Severity >= Event.EventSeverity.Fatal )
			{
				throw new XeusException( myEvent.Message ) ;
			}
#endif
		}
	}
}