using System.Windows.Threading ;
using xeus2.xeus.Core ;

namespace xeus2.xeus.Middle
{
	internal class Muc
	{
		private static Muc _instance = new Muc() ;

        UI.Muc _muc = new UI.Muc();

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
            _muc.AddMuc(service, nick, password);
			_muc.Show() ;
		}
	}
}