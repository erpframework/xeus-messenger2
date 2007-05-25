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

		public override string ToString()
		{
			return Name ;
		}

		public static string GetCategoryText( string category )
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