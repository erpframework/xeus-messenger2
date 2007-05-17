using System.Windows.Threading ;
using agsXMPP.protocol.client ;
using agsXMPP.protocol.iq.disco ;
using xeus2.Properties ;
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
			App.InvokeSafe( DispatcherPriority.Background,
			                new ServiceItemInfoCallback( OnServiceItemInfo ), discoItem, info ) ;
		}

		public void OnServiceItem( object sender, DiscoItem discoItem, DiscoItem parent )
		{
			App.InvokeSafe( DispatcherPriority.Background,
			                new ServiceItemCallback( OnServiceItem ), discoItem, parent ) ;
		}

		public void OnServiceItemError( object sender, IQ iq )
		{
			App.InvokeSafe( DispatcherPriority.Background,
			                new OnServiceItemErrorCallback( OnServiceItemError ), iq ) ;
		}

		public void OnServiceItemError( IQ iq )
		{
			EventInfo eventInfo = new EventInfo( string.Format( Resources.Error_ServiceDiscoFailed, iq.From, iq.Error.Code ) ) ;
			Events.Instance.OnEvent( eventInfo ) ;
		}

		public void OnServiceItemInfo( DiscoItem discoItem, DiscoInfo info )
		{
			lock ( _syncObject )
			{
				Service service = FindService( discoItem ) ;

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
				Service service = FindService( discoItem ) ;
				Service parentService = null ;

				if ( parent != null )
				{
					parentService = FindService( parent ) ;
				}

				if ( service == null )
				{
					if ( parent == null )
					{
						Add( new Service( discoItem ) ) ;
					}
					else
					{
						bool isCommand = ( parentService != null
						                   && parentService.DiscoInfo != null
						                   && parentService.DiscoInfo.HasFeature( agsXMPP.Uri.COMMANDS ) ) ;

						lock ( parentService.Services._syncObject )
						{
							parentService.Services.Add( new Service( discoItem, isCommand ) ) ;
						}
					}
				}
			}
		}

		// unsafe, lock when calling
		public Service FindService( DiscoItem discoItem )
		{
			foreach ( Service item in Items )
			{
				if ( JidUtil.CompareDiscoItem( item.DiscoItem, discoItem ) )
				{
					return item ;
				}

				lock ( item.Services._syncObject )
				{
					Service service = item.Services.FindService( discoItem ) ;

					if ( service != null )
					{
						return service ;
					}
				}
			}

			return null ;
		}
	}
}