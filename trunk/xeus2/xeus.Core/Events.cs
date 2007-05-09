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
			App.Current.Dispatcher.Invoke( DispatcherPriority.Background,
			                               new EventItemCallback( OnEvent ), myEvent ) ;
		}

		public void OnEvent( Event myEvent )
		{
		}
	}
}