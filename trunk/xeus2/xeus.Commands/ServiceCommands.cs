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

		private static RoutedUICommand _next =
			new RoutedUICommand( "Next", "ServiceCommandNext", typeof ( ServiceCommands ) ) ;

		private static RoutedUICommand _complete =
			new RoutedUICommand( "Complete", "ServiceCommandComplete", typeof ( ServiceCommands ) ) ;

		private static RoutedUICommand _cancel =
			new RoutedUICommand( "Cancel", "ServiceCommandCancel", typeof ( ServiceCommands ) ) ;

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

		public static RoutedUICommand Next
		{
			get
			{
				return _next ;
			}
		}

		public static RoutedUICommand Complete
		{
			get
			{
				return _complete ;
			}
		}

		public static RoutedUICommand Cancel
		{
			get
			{
				return _cancel ;
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

			window.CommandBindings.Add(
				new CommandBinding( _next, ExecuteRunNext, CanExecuteRunNext ) ) ;

			window.CommandBindings.Add(
				new CommandBinding( _complete, ExecuteRunComplete, CanExecuteRunComplete ) ) ;

			window.CommandBindings.Add(
				new CommandBinding( _cancel, ExecuteRunCancel, CanExecuteRunCancel ) ) ;
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
			e.CanExecute = ( command != null
			                 && command.Command.Actions != null
			                 && command.Command.Actions.Previous ) ;
		}

		public static void ExecuteRunPrevious( object sender, ExecutedRoutedEventArgs e )
		{
			ServiceCommandExecution command = e.Parameter as ServiceCommandExecution ;

			Account.Instance.ServiceCommandPrevious( command ) ;

			e.Handled = true ;
		}

		public static void CanExecuteRunNext( object sender, CanExecuteRoutedEventArgs e )
		{
			ServiceCommandExecution command = e.Parameter as ServiceCommandExecution ;

			e.Handled = true ;
			e.CanExecute = ( command != null
			                 && command.Command.Actions != null
			                 && command.Command.Actions.Next ) ;
		}

		public static void ExecuteRunNext( object sender, ExecutedRoutedEventArgs e )
		{
			ServiceCommandExecution command = e.Parameter as ServiceCommandExecution ;

			Account.Instance.ServiceCommandNext( command ) ;

			e.Handled = true ;
		}

		public static void CanExecuteRunComplete( object sender, CanExecuteRoutedEventArgs e )
		{
			ServiceCommandExecution command = e.Parameter as ServiceCommandExecution ;

			e.Handled = true ;
			e.CanExecute = ( command != null
			                 && command.Command.Actions != null
			                 && command.Command.Actions.Complete ) ;
		}

		public static void ExecuteRunComplete( object sender, ExecutedRoutedEventArgs e )
		{
			ServiceCommandExecution command = e.Parameter as ServiceCommandExecution ;

			Account.Instance.ServiceCommandComplete( command ) ;

			e.Handled = true ;
		}

		public static void CanExecuteRunCancel( object sender, CanExecuteRoutedEventArgs e )
		{
			ServiceCommandExecution command = e.Parameter as ServiceCommandExecution ;

			e.Handled = true ;
			e.CanExecute = ( command != null
			                 && command.Command != null
			                 && command.Command.Status == Status.executing ) ;
		}

		public static void ExecuteRunCancel( object sender, ExecutedRoutedEventArgs e )
		{
			ServiceCommandExecution command = e.Parameter as ServiceCommandExecution ;

			Account.Instance.ServiceCommandCancel( command ) ;

			e.Handled = true ;
		}
	}
}