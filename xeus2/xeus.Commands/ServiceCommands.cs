using System.Windows ;
using System.Windows.Input ;
using agsXMPP.protocol.extensions.commands ;
using xeus2.xeus.Core ;
using xeus2.xeus.Middle ;
using xeus2.xeus.Utilities ;

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

		private static RoutedUICommand _joinMuc =
			new RoutedUICommand( "JoinMuc", "ServiceCommandJoinMuc", typeof ( ServiceCommands ) ) ;

		private static RoutedUICommand _mucInfo =
			new RoutedUICommand( "MucInfo", "ServiceCommandMucInfo", typeof ( ServiceCommands ) ) ;

		private static RoutedUICommand _displayServices =
			new RoutedUICommand( "DisplayServices", "ServicesDisplay", typeof ( ServiceCommands ) ) ;

		private static RoutedUICommand _discoveryServices =
			new RoutedUICommand( "DiscoveryServices", "ServicesDiscovery", typeof ( ServiceCommands ) ) ;

		private static RoutedUICommand _stopDiscoveryServices =
			new RoutedUICommand( "StopDiscoveryServices", "StopServicesDiscovery", typeof ( ServiceCommands ) ) ;

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

		public static RoutedUICommand JoinMuc
		{
			get
			{
				return _joinMuc ;
			}
		}

		public static RoutedUICommand MucInfo
		{
			get
			{
				return _mucInfo ;
			}
		}

		public static RoutedUICommand DisplayServices
		{
			get
			{
				return _displayServices ;
			}
		}

		public static RoutedUICommand DiscoveryServices
		{
			get
			{
				return _discoveryServices ;
			}
		}

		public static RoutedUICommand StopDiscoveryServices
		{
			get
			{
				return _stopDiscoveryServices ;
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

			window.CommandBindings.Add(
				new CommandBinding( _joinMuc, ExecuteJoinMuc, CanExecuteJoinMuc ) ) ;

			window.CommandBindings.Add(
				new CommandBinding( _mucInfo, ExecuteMucInfo, CanExecuteMucInfo ) ) ;

			window.CommandBindings.Add(
				new CommandBinding( _displayServices, ExecuteDisplayServices, CanExecuteDisplayServices ) ) ;

			window.CommandBindings.Add(
				new CommandBinding( _discoveryServices, ExecuteDiscoveryServices, CanExecuteDiscoveryServices ) ) ;

			window.CommandBindings.Add(
				new CommandBinding( _stopDiscoveryServices, ExecuteStopDiscoveryServices, CanExecuteStopDiscoveryServices ) ) ;
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

			Service command = CommandExecutor.Instance.ChooseCommand( service ) ;

			if ( command != null )
			{
				Account.Instance.ServiceCommand( command ) ;
			}

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

		public static void CanExecuteJoinMuc( object sender, CanExecuteRoutedEventArgs e )
		{
			if ( e.Parameter is Service )
			{
				e.CanExecute = ( ( ( Service )e.Parameter ).IsChatRoom ) ;
			}
			else if ( e.Parameter is string )
			{
				e.CanExecute = ( Validation.IsChatRoomValid( ( ( string )e.Parameter ) ) ) ;
			}

			e.Handled = true ;
		}

		public static void ExecuteJoinMuc( object sender, ExecutedRoutedEventArgs e )
		{
			if ( e.Parameter is Service )
			{
				Account.Instance.JoinMuc( ( Service )e.Parameter ) ;
			}
			else if ( e.Parameter is string )
			{
				Account.Instance.JoinMuc( ( string )e.Parameter ) ;
			}

			e.Handled = true ;
		}

		public static void CanExecuteMucInfo( object sender, CanExecuteRoutedEventArgs e )
		{
			Service service = e.Parameter as Service ;

			e.Handled = true ;
			e.CanExecute = ( service != null && service.IsChatRoom ) ;
		}

		public static void ExecuteMucInfo( object sender, ExecutedRoutedEventArgs e )
		{
			Service service = e.Parameter as Service ;

			Middle.MucInfo.Instance.DisplayMucInfo( service );

			e.Handled = true ;
		}

		public static void CanExecuteDisplayServices( object sender, CanExecuteRoutedEventArgs e )
		{
			e.Handled = true ;
			e.CanExecute = true ;
		}

		public static void ExecuteDisplayServices( object sender, ExecutedRoutedEventArgs e )
		{
			Middle.Services.Instance.Display();

			e.Handled = true ;
		}

		public static void CanExecuteDiscoveryServices( object sender, CanExecuteRoutedEventArgs e )
		{
			e.Handled = true ;
			e.CanExecute = ( e.Parameter is string ) ;
		}

		public static void ExecuteDiscoveryServices( object sender, ExecutedRoutedEventArgs e )
		{
			string jid = e.Parameter as string ;

			Account.Instance.Discovery( jid );

			e.Handled = true ;
		}

		public static void CanExecuteStopDiscoveryServices( object sender, CanExecuteRoutedEventArgs e )
		{
			e.Handled = true ;
			e.CanExecute = true ;
		}

		public static void ExecuteStopDiscoveryServices( object sender, ExecutedRoutedEventArgs e )
		{
			Account.Instance.StopDiscovery();

			e.Handled = true ;
		}
	}
}