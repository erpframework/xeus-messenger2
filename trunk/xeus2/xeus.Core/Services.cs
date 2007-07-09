using System ;
using System.Collections.Generic ;
using System.Windows.Threading ;
using agsXMPP ;
using agsXMPP.protocol.client ;
using agsXMPP.protocol.iq.disco ;
using agsXMPP.Xml.Dom ;
using xeus2.Properties ;

namespace xeus2.xeus.Core
{
	internal class Services : ObservableCollectionDisp< Service >
	{
		private static Services _instance = new Services() ;

		private Dictionary< string, Service > _allServices = new Dictionary< string, Service >() ;

		private delegate void ServiceItemCallback( IList<DiscoItem> discoItems, DiscoItem parent ) ;

		private delegate void ServiceItemInfoCallback( DiscoItem discoItem, DiscoInfo info ) ;

		private delegate void OnServiceItemErrorCallback( IQ iq ) ;

		private delegate void OnCommandsItemInfoCallback( DiscoItem discoItem, IQ iq ) ;

		private string _sessionKey = string.Empty ;

        private ObservableCollectionDisp<Service> _filteredServices = new ObservableCollectionDisp<Service>();

		public static Services Instance
		{
			get
			{
				return _instance ;
			}
		}

		public static ServiceCategories Categories
		{
			get
			{
				return _categories ;
			}
		}

        ObservableCollectionDisp<Service> _allServicesCollection = new ObservableCollectionDisp<Service>();

        public ObservableCollectionDisp<Service> AllServices
	    {
            get
	        {
	            return _allServicesCollection;
	        }
	    }

	    public string SessionKey
		{
			get
			{
				return _sessionKey ;
			}
		}

        public ObservableCollectionDisp<Service> FilteredServices
	    {
	        get
	        {
	            foreach (Service service in _allServices.Values)
	            {
	                _filteredServices.Add(service);
	            }
	            return _filteredServices;
	        }
	    }

	    public new void Clear()
		{
			lock ( _syncObject )
			{
				_sessionKey = Guid.NewGuid().ToString() ;

				_allServices.Clear() ;
                _allServicesCollection.Clear();

				Categories.Clear() ;

				base.Clear() ;
			}
		}

		private static ServiceCategories _categories = new ServiceCategories() ;

		public void OnCommandsItemInfo( object sender, DiscoItem discoItem, IQ iq )
		{
            if (_sessionKey == string.Empty)
            {
                return;
            }

			App.InvokeSafe( DispatcherPriority.Background,
			                new OnCommandsItemInfoCallback( OnCommandsItemInfo ), discoItem, iq ) ;
		}

		public void OnServiceItemInfo( object sender, DiscoItem discoItem, DiscoInfo info )
		{
			if ( _sessionKey == string.Empty )
			{
				return ;
			}

			App.InvokeSafe( DispatcherPriority.Background,
			                new ServiceItemInfoCallback( OnServiceItemInfo ), discoItem, info ) ;
		}

		public void OnServiceItem( object sender, IList<DiscoItem> discoItems, DiscoItem parent )
		{
            if (_sessionKey == string.Empty)
            {
                return;
            }

            App.InvokeSafe(DispatcherPriority.Background,
			                new ServiceItemCallback( OnServiceItems ), discoItems, parent ) ;
		}

		public void OnServiceItemError( object sender, IQ iq )
		{
			App.InvokeSafe( DispatcherPriority.Background,
			                new OnServiceItemErrorCallback( OnServiceItemError ), iq ) ;
		}

		public void OnServiceItemError( IQ iq )
		{
			EventInfo eventInfo = new EventInfo( string.Format( Resources.Error_ServiceDiscoFailed, iq.From, iq.Error.Code ) ) ;
			Events.Instance.OnEvent( this, eventInfo ) ;
		}

		public void OnCommandsItemInfo( DiscoItem discoItem, IQ iq )
		{
			lock ( _syncObject )
			{
				Service service = FindService( discoItem ) ;

				if ( service != null )
				{
					foreach ( Node node in iq.Query.ChildNodes )
					{
						DiscoItem item = node as DiscoItem ;

						if ( item != null )
						{
							Service command = FindService( item ) ;

							if ( command == null )
							{
								// it is not in hierarchy
								command = new Service( item, false ) ;

								_allServices.Add( command.Key, command ) ;
                                _allServicesCollection.Add(command);

								Account.Instance.AddDiscoInfoPrioritized( item ) ;
							}

							lock ( service.Commands._syncObject )
							{
								service.Commands.Add( command ) ;
							}
						}
					}
				}
			}
		}

		private void OnServiceItemInfo( DiscoItem discoItem, DiscoInfo info )
		{
			lock ( _syncObject )
			{
				Service service = FindService( discoItem ) ;

				if ( service != null )
				{
					service.DiscoInfo = info ;

					if ( service.IsToplevel )
					{
						_categories.AddService( service ) ;
					}
				}
			}
		}

		private void OnServiceItems( IList<DiscoItem> discoItems, DiscoItem parent )
		{
			lock ( _syncObject )
			{
                List<Service> services = new List<Service>(discoItems.Count);
                Service parentService = null;

                foreach (DiscoItem discoItem in discoItems)
                {
                    Service service = FindService(discoItem);

                    if (parent != null)
                    {
                        parentService = FindService(parent);
                    }

                    if (service == null)
                    {
                        Service newService = new Service(discoItem, (parent == null));
                        _allServices.Add(newService.Key, newService);
                        _allServicesCollection.Add(newService);
                        services.Add(newService);
                    }
                }

                if (parent == null)
                {
                    Add(services);
                }
                else
                {
                    if (parentService != null)
                    {
                        lock (parentService.Services._syncObject)
                        {
                            parentService.Services.Add(services);
                        }
                    }
                }
			}
		}

		// unsafe, lock when calling
		public Service FindService( DiscoItem discoItem )
		{
			Service service ;

			_allServices.TryGetValue( Service.GetKey( discoItem ), out service ) ;

			return service ;
		}

		// unsafe, lock when calling
		public Service FindService( Jid jid )
		{
			Service service ;

			_allServices.TryGetValue( Service.GetKey( jid ), out service ) ;

			return service ;
		}

		public void StopSession()
		{
			_sessionKey = string.Empty ;
		}
	}
}