using System ;
using System.Windows.Threading ;
using agsXMPP ;
using agsXMPP.protocol.client ;
using agsXMPP.protocol.iq.disco ;
using xeus2.xeus.Utilities ;

namespace xeus2.xeus.Core
{
	internal class Services : ObservableCollectionDisp< Service >
	{
		private delegate void ServiceItemCallback( DiscoItem discoItem, DiscoItem parent ) ;
		private delegate void ServiceItemInfoCallback( DiscoItem discoItem, DiscoInfo info ) ;
		private delegate void OnServiceItemErrorCallback( IQ iq ) ;

		private static Services _instance = new Services() ;

		public static Services Instance
		{
			get
			{
				return _instance ;
			}
		}

		public void OnServiceItemInfo( object sender, DiscoItem discoItem, DiscoInfo info )
		{
			App.Current.Dispatcher.Invoke( DispatcherPriority.Background,
			                               new ServiceItemInfoCallback( OnServiceItemInfo ), discoItem, info ) ;
		}

		public void OnServiceItem( object sender, DiscoItem discoItem, DiscoItem parent )
		{
			App.Current.Dispatcher.Invoke( DispatcherPriority.Background,
			                               new ServiceItemCallback( OnServiceItem ), discoItem, parent ) ;
		}

		public void OnServiceItemError( object sender, IQ iq )
		{
			App.Current.Dispatcher.Invoke( DispatcherPriority.Background,
			                               new OnServiceItemErrorCallback( OnServiceItemError ), iq ) ;
		}

		public void OnServiceItemError( IQ iq )
		{
			lock ( _syncObject )
			{
				Service service = FindService( iq.From ) ;

				if ( service != null )
				{
					service.ErrorIq = iq ;
				}
			}
		}

		public void OnServiceItemInfo( DiscoItem discoItem, DiscoInfo info )
		{
			lock ( _syncObject )
			{
				Service service = FindService( discoItem.Jid ) ;

				if ( service != null )
				{
					service.DiscoInfo = info ;
				}
			}
		}

		public void OnServiceItem( DiscoItem discoItem, DiscoItem parent )
		{
			lock ( _syncObject )
			{
				Service service = FindService( discoItem.Jid ) ;
				Service parentService = null ;

				if ( parent != null )
				{
					parentService = FindService( parent.Jid ) ;
				}

				if ( service == null )
				{
					if ( parent == null )
					{
						Add( new Service( discoItem ) ) ;
					}
					else
					{
						parentService.Services.Add( new Service( discoItem ) ) ;
					}
				}
				else
				{
					// service.DiscoInfo = discoInfo ;
					return ;
				}
			}
		}

		public Service FindService( Jid jid )
		{
			foreach ( Service item in Items )
			{
				if ( JidUtil.Equals( jid, item.Jid ) )
				{
					return item ;
				}

				lock ( item.Services._syncObject )
				{
					Service service = item.Services.FindService( jid ) ;

					if ( service!= null )
					{
						return service ;
					}
				}
			}

			return null ;
		}
	}
}