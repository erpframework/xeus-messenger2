using System.Windows.Threading ;
using xeus2.xeus.Core ;

namespace xeus2.xeus.Middle
{
	internal class Muc
	{
		private static Muc _instance = new Muc() ;

		private delegate void MucLoginCallback( Service servie, string nick, string password ) ;

		public static Muc Instance
		{
			get
			{
				return _instance ;
			}
		}

		public void DisplayMuc( Service service, string nick, string password )
		{
            App.InvokeSafe(App._dispatcherPriority,
			                new MucLoginCallback( DisplayMucInternal ), service, nick, password ) ;

		}

		protected void DisplayMucInternal( Service service, string nick, string password )
		{
			UI.Muc muc = new UI.Muc( service, nick, password );

			muc.Show() ;
		}
	}
}