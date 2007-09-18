using System.Windows.Threading ;

namespace xeus2.xeus.Core
{
	internal class Events : ObservableCollectionDisp< Event >
	{
		public delegate void EventItemCallback( object sender, Event myEvent ) ;
		public event EventItemCallback OnEventRaised ;

		private static readonly Events _instance = new Events() ;

	    private const uint _maxEvents = 100;

		public static Events Instance
		{
			get
			{
				return _instance ;
			}
		}

        public void OnEvent(object sender, Event myEvent, DispatcherPriority priority)
        {
            App.InvokeSafe(DispatcherPriority.ApplicationIdle,
                            new EventItemCallback(OnEventInternal), sender, myEvent);
        }

		public void OnEvent( object sender, Event myEvent )
		{
		    OnEvent(sender, myEvent, DispatcherPriority.Background);
		}

		public void OnEventInternal( object sender, Event myEvent )
		{
            lock (_syncObject)
            {
                Add(myEvent);

                if (Count > _maxEvents)
                {
                    RemoveAt(0);
                }
            }

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