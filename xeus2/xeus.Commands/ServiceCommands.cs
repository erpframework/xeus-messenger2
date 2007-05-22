using System.Windows ;
using System.Windows.Input ;
using agsXMPP.protocol.extensions.commands ;
using xeus2.xeus.Core ;

namespace xeus2.xeus.Commands
{
	public static class ServiceCommands
	{
		private static RoutedUICommand _register =
			new RoutedUICommand( "Register service", "ServiceRegister", typeof ( ServiceCommands ) ) ;

		private static RoutedUICommand _search =
			new RoutedUICommand( "Search service", "ServiceSearch", typeof ( ServiceCommands ) ) ;

		private static RoutedUICommand _run =
			new RoutedUICommand( "Run command", "ServiceCommandRun", typeof ( ServiceCommands ) ) ;

		private static RoutedUICommand _previous =
			new RoutedUICommand( "Previous", "ServiceCommandPrevious", typeof ( ServiceCommands ) ) ;

		public static RoutedUICommand Register
		{
			get
			{
				return _register ;
			}
		}

		public static RoutedUICommand Search
		{
			get
			{
				return _search ;
			}
		}

		public static RoutedUICommand Run
		{
			get
			{
				return _run ;
			}
		}

		public static RoutedUICommand Previous
		{
			get
			{
				return _previous ;
			}
		}

		public static void RegisterCommands( Window window )
		{
			window.CommandBindings.Add(
				new CommandBinding( _register, ExecuteRegister, CanExecuteRegister ) ) ;

			window.CommandBindings.Add(
				new CommandBinding( _search, ExecuteSearch, CanExecuteSearch ) ) ;

			window.CommandBindings.Add(
				new CommandBinding( _run, ExecuteRun, CanExecuteRun ) ) ;

			window.CommandBindings.Add(
				new CommandBinding( _previous, ExecuteRunPrevious, CanExecuteRunPrevious ) ) ;
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

		public static void CanExecuteSearch( object sender, CanExecuteRoutedEventArgs e )
		{
			Service service = e.Parameter as Service ;

			e.Handled = true ;
			e.CanExecute = ( service != null && service.IsSearchable ) ;
		}

		public static void ExecuteSearch( object sender, ExecutedRoutedEventArgs e )
		{
			Service service = e.Parameter as Service ;

			Account.Instance.GetServiceSearch( service ) ;

			e.Handled = true ;
		}

		public static void CanExecuteRun( object sender, CanExecuteRoutedEventArgs e )
		{
			Service service = e.Parameter as Service ;

			e.Handled = true ;
			e.CanExecute = ( service != null && service.IsCommand ) ;
		}

		public static void ExecuteRun( object sender, ExecutedRoutedEventArgs e )
		{
			Service service = e.Parameter as Service ;

			Account.Instance.ServiceCommand( service ) ;

			e.Handled = true ;
		}

		public static void CanExecuteRunPrevious( object sender, CanExecuteRoutedEventArgs e )
		{
			ServiceCommandExecution command = e.Parameter as ServiceCommandExecution ;

			e.Handled = true ;
			e.CanExecute = ( command != null && command.Command.Actions.Previous ) ;
		}

		public static void ExecuteRunPrevious( object sender, ExecutedRoutedEventArgs e )
		{
			ServiceCommandExecution command = e.Parameter as ServiceCommandExecution ;

			Account.Instance.ServiceCommandPrevious( command ) ;

			e.Handled = true ;
		}
	}
}