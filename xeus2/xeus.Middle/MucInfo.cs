using System.Windows.Threading ;
using xeus2.xeus.Core ;
using xeus2.xeus.UI ;

namespace xeus2.xeus.Middle
{
	internal class MucInfo
	{
		private delegate void DisplayCallback( Service service ) ;

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
	}
}