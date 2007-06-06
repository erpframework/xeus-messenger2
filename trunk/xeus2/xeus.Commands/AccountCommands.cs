using System.Windows ;
using System.Windows.Input ;
using xeus2.xeus.Middle ;

namespace xeus2.xeus.Commands
{
	public static class AccountCommands
	{
		private static RoutedUICommand _displayLogin =
			new RoutedUICommand( "Display Login", "DisplayLogin", typeof ( AccountCommands ) ) ;

		public static RoutedUICommand DisplayLogin
		{
			get
			{
				return _displayLogin ;
			}
		}

		public static void RegisterCommands( Window window )
		{
			window.CommandBindings.Add(
				new CommandBinding( _displayLogin, ExecuteDisplayLogin, CanExecuteDisplayLogin ) ) ;
		}

		public static void CanExecuteDisplayLogin( object sender, CanExecuteRoutedEventArgs e )
		{
			e.Handled = true ;
			e.CanExecute = true ;
		}

		public static void ExecuteDisplayLogin( object sender, ExecutedRoutedEventArgs e )
		{
			Login.Instance.DisplayLogin() ;

			e.Handled = true ;
		}
	}
}