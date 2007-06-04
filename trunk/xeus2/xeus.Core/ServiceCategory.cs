using System.Windows.Media.Imaging ;
using xeus.Data ;
using xeus2.xeus.Utilities ;

namespace xeus2.xeus.Core
{
	internal class ServiceCategory
	{
		private Services _services = new Services() ;
		private string _name ;

		public ServiceCategory( string name )
		{
			_name = name ;
		}

		public Services Items
		{
			get
			{
				return _services ;
			}
		}


		public Services Services
		{
			get
			{
				return _services ;
			}
		}

		public string Name
		{
			get
			{
				return _name ;
			}
		}

		public string Text
		{
			get
			{
				return GetCategoryText( Name ) ;
			}
		}

		public string Description
		{
			get
			{
				return GetCategoryDescription( Name ) ;
			}
		}

		public override string ToString()
		{
			return Name ;
		}

		public BitmapImage Image
		{
			get
			{
				return GetCategoryImage( Name ) ;
			}
		}

		private static BitmapImage GetCategoryImage( string category )
		{
			switch ( category )
			{
				case "conference":
					{
						return Storage.GetServiceImage( "service_conference.png" ) ;
					}
				case "directory":
					{
						return Storage.GetServiceImage( "service_search.png" ) ;
					}
				case "gateway":
					{
						return Storage.GetServiceImage( "service_gateway.png" ) ;
					}
				case "proxy":
					{
						return Storage.GetServiceImage( "service_proxy.png" ) ;
					}
				case "pubsub":
					{
						return Storage.GetServiceImage( "service_pubsub.png" ) ;
					}
				case "store":
					{
						return Storage.GetServiceImage( "service_store.png" ) ;
					}
				case "headline":
					{
						return Storage.GetServiceImage( "service_headline.png" ) ;
					}
				case "server":
				case "automation":
				case "collaboration":
				case "component":
				default:
					{
						return Storage.GetDefaultServiceImage() ;
					}
			}
		}

		private static string GetCategoryDescription( string category )
		{
			switch ( category )
			{
				case "account":
					{
						return
							"Used by a server when responding to a disco request sent to the bare JID (user@host addresss) of an account hosted by the server" ;
					}
				case "auth":
					{
						return "Components that provide authentication services within a server implementation" ;
					}
				case "automation":
					{
						return "Entities and nodes that provide automated or programmed interaction" ;
					}
				case "client":
					{
						return "Different types of clients, mostly for instant messaging" ;
					}
				case "collaboration":
					{
						return "Services that enable multiple individuals to work together in real time" ;
					}
				case "component":
					{
						return "Services that are internal to server implementations and not normally exposed outside a server" ;
					}
				case "conference":
					{
						return "Online conference services such as multi-user chatroom services" ;
					}
				case "directory":
					{
						return
							"Information retrieval services that enable users to search online directories or otherwise be informed about the existence of other XMPP entities" ;
					}
				case "gateway":
					{
						return "Translators between Jabber/XMPP services and non-XMPP services" ;
					}
				case "headline":
					{
						return
							"Services that provide real-time news or information (often but not necessarily in a message of type \"headline\")" ;
					}
				case "hierarchy":
					{
						return "Used to describe nodes within a hierarchy of nodes; the \"branch\" and \"leaf\" types are exhaustive" ;
					}
				case "proxy":
					{
						return
							"Servers or services that act as special-purpose proxies or intermediaries between two or more XMPP endpoints" ;
					}
				case "pubsub":
					{
						return "Publish and Subscribe various information" ;
					}
				case "server":
					{
						return "Jabber/XMPP servers" ;
					}
				case "store":
					{
						return "Internal server components that provide data storage and retrieval services" ;
					}
				default:
					{
						return TextUtil.ToTitleCase( category ) ;
					}
			}
		}

		private static string GetCategoryText( string category )
		{
			switch ( category )
			{
				case "account":
					{
						return "Account" ;
					}
				case "auth":
					{
						return "Authentication" ;
					}
				case "automation":
					{
						return "Automation" ;
					}
				case "client":
					{
						return "Client" ;
					}
				case "collaboration":
					{
						return "Collaboration" ;
					}
				case "component":
					{
						return "Internal Service" ;
					}
				case "conference":
					{
						return "Conference, Multi-User Chat" ;
					}
				case "directory":
					{
						return "Search" ;
					}
				case "gateway":
					{
						return "Transport, Gateway" ;
					}
				case "headline":
					{
						return "Real-Time News" ;
					}
				case "hierarchy":
					{
						return "Hierarchy" ;
					}
				case "proxy":
					{
						return "Special-Purpose Proxy" ;
					}
				case "pubsub":
					{
						return "Publish-Subscribe" ;
					}
				case "server":
					{
						return "XMPP Server" ;
					}
				case "store":
					{
						return "Storage" ;
					}
				default:
					{
						return TextUtil.ToTitleCase( category ) ;
					}
			}
		}
	}
}