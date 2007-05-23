using System.Windows.Threading ;
using xeus2.xeus.Core ;
using xeus2.xeus.UI ;

namespace xeus2.xeus.Middle
{
	internal class MucInfo
	{
		private delegate void DisplayCallback( Service service ) ;
		private delegate void MucLoginCallback( Service servie, string forceNick ) ;

		private static MucInfo _instance = new MucInfo() ;

		public static MucInfo Instance
		{
			get
			{
				return _instance ;
			}
		}

		public void DisplayMucInfo( Service service )
		{
			App.InvokeSafe( DispatcherPriority.Normal,
			                new DisplayCallback( DisplayMucInfoInternal ), service ) ;
		}

		protected void DisplayMucInfoInternal( Service service )
		{
			RoomInfo roomInfo = new RoomInfo( service ) ;
			roomInfo.DataContext = service ;
			roomInfo.Show() ;
		}

		public void MucLogin( Service service, string forceNick )
		{
			App.InvokeSafe( DispatcherPriority.Normal,
			                new MucLoginCallback( MucLoginInternal ), service, forceNick ) ;
		}

		protected static void MucLoginInternal( Service service, string forceNick )
		{
			RoomLogin roomLogin = new RoomLogin( service, forceNick );

			roomLogin.DataContext = service ;
			roomLogin.Show() ;
		}
	}
}