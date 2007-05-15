using System ;
using System.Windows ;
using System.Windows.Threading ;

namespace xeus2
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			//xeus2.Properties.Resources.Culture = new CultureInfo( "en-US" ) ;
			//string h = xeus2.Properties.Resources.Event_PresenceChange ;
		}

		public static bool CheckAccessSafe()
		{
			if ( Current == null )
			{
				return false ;
			}

			return Current.Dispatcher.CheckAccess() ;
		}

		public static void InvokeSafe( DispatcherPriority priority, Delegate method, object arg )
		{
			if ( Current == null )
			{
				return ;
			}

			Current.Dispatcher.Invoke( priority, method, arg ) ;
		}

		public static void InvokeSafe( DispatcherPriority priority, Delegate method, object arg, params object[] args )
		{
			if ( Current == null )
			{
				return ;
			}

			Current.Dispatcher.Invoke( priority, method, arg, args ) ;
		}

		public static void InvokeSafe( DispatcherPriority priority, Delegate method, params object[] args )
		{
			if ( Current == null )
			{
				return ;
			}

			Current.Dispatcher.Invoke( priority, method, args ) ;
		}
	}
}