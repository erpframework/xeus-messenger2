using System.Windows ;
using System.Windows.Input ;
using xeus2.xeus.Core ;

namespace xeus2.xeus.Commands
{
	public static class ServiceCommands
	{
		private static RoutedUICommand _register =
			new RoutedUICommand( "Register service", "ServiceRegister", typeof ( ServiceCommands ) ) ;

		public static RoutedUICommand Register
		{
			get
			{
				return _register ;
			}
		}

		public static void RegisterCommands( Window window )
		{
			window.CommandBindings.Add(
				new CommandBinding( _register, ExecuteRegister, CanExecuteRegister ) ) ;
		}

		public static void CanExecuteRegister( object sender, CanExecuteRoutedEventArgs e )
		{
			Service service = e.Parameter as Service ;

			e.Handled = true ;
			e.CanExecute = ( service != null && service.IsRegistrable ) ;
		}

		public static void ExecuteRegister( object sender, ExecutedRoutedEventArgs e )
		{
			Service service = e.Parameter as Service ;

			Account.Instance.GetService( service ) ;

			e.Handled = true ;
		}
	}
}