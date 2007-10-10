using xeus2.Properties ;
using xeus2.xeus.Core ;

namespace xeus2.xeus.Middle
{
	internal class Login
	{
		private static Login _instance = new Login() ;

		public static Login Instance
		{
			get
			{
				return _instance ;
			}
		}

		public void DisplayLogin()
		{
			UI.Login login = new UI.Login() ;

		    login.Activate();
			if ( ( bool ) login.ShowDialog() )
			{
				Settings.Default.XmppUserName = login.User ;
				Settings.Default.XmppServer = login.Server ;
				Settings.Default.XmppPassword = login.Password ;

				Account.Instance.Login() ;
			}
		}
	}
}