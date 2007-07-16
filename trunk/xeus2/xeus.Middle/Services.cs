using System ;
using System.ComponentModel ;
using System.Windows ;
using System.Windows.Threading ;
using xeus2.xeus.Commands ;
using xeus2.xeus.Core ;
using xeus2.xeus.UI ;

namespace xeus2.xeus.Middle
{
	internal class Services : WindowManager< string, ServiceWindow >
	{
		private static Services _instance = new Services() ;

		private delegate void DisplayCallback() ;

		public static Services Instance
		{
			get
			{
				return _instance ;
			}
		}

		protected void DisplayInternal()
		{
			ServiceWindow serviceWindow = GetWindow( String.Empty ) ;

			if ( serviceWindow == null )
			{
				serviceWindow = new ServiceWindow() ;
				serviceWindow.Closing += new CancelEventHandler( serviceWindow_Closing ) ;
				AddWindow( String.Empty, serviceWindow ) ;

				if ( Core.Services.Instance.Count == 0 )
				{
					ServiceCommands.DiscoveryServices.Execute( string.Empty, null ) ;
				}
			}
			else
			{
				serviceWindow.Activate() ;
			}

			serviceWindow.DataContext = Account.Instance ;
			serviceWindow.Show() ;
		}

		public void Display()
		{
            App.InvokeSafe(App._dispatcherPriority,
			                new DisplayCallback( DisplayInternal ) ) ;
		}

		private void serviceWindow_Closing( object sender, CancelEventArgs e )
		{
			ServiceCommands.StopDiscoveryServices.Execute( null, null ) ;

			RemoveWindow( string.Empty ) ;

			( ( Window ) sender ).Closing -= serviceWindow_Closing ;
		}
	}
}