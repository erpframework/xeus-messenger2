using System.Windows.Threading ;
using agsXMPP.protocol.iq.disco ;

namespace xeus2.xeus.Core
{
	internal class Services : ObservableCollectionDisp< Service >
	{
		private delegate void ServiceItemCallback( DiscoInfo discoInfo ) ;

		private static Services _instance = new Services() ;

		public static Services Instance
		{
			get
			{
				return _instance ;
			}
		}

		public void OnServiceItem( object sender, DiscoInfo discoInfo )
		{
			App.Current.Dispatcher.Invoke( DispatcherPriority.Background,
			                               new ServiceItemCallback( OnServiceItem ), discoInfo ) ;
		}

		public void OnServiceItem( DiscoInfo discoInfo )
		{
			lock ( _syncObject )
			{
				Service service = FindService( discoInfo.Node ) ;

				if ( service == null )
				{
					Add( new Service( discoInfo ) ) ;
				}
				else
				{
					// todo:
				}
			}
		}

		// unsafe - use lock in calling code
		private Service FindService( string name )
		{
			foreach ( Service item in Items )
			{
				if ( item.Name == name )
				{
					return item ;
				}
			}

			return null ;
		}
	}
}