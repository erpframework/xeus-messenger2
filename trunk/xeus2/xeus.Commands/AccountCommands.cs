using System.Windows ;
using System.Windows.Input ;
using xeus2.xeus.Core;
using xeus2.xeus.Middle ;

namespace xeus2.xeus.Commands
{
	public static class AccountCommands
	{
		private static readonly RoutedUICommand _displayLogin =
			new RoutedUICommand( "Display Login", "DisplayLogin", typeof ( AccountCommands ) ) ;

        private static readonly RoutedUICommand _history =
            new RoutedUICommand("Display History", "DisplayHistory", typeof(AccountCommands));

        private static readonly RoutedUICommand _displayMucMarks =
            new RoutedUICommand("Display MUC Bookmarks", "DisplayMUCBookmarks", typeof(AccountCommands));

        public static RoutedUICommand DisplayLogin
		{
			get
			{
				return _displayLogin ;
			}
		}

	    public static RoutedUICommand History
	    {
	        get
	        {
	            return _history;
	        }
	    }

	    public static RoutedUICommand DisplayMucMarks
	    {
	        get
	        {
	            return _displayMucMarks;
	        }
	    }

	    public static void RegisterCommands( Window window )
		{
			window.CommandBindings.Add(
				new CommandBinding( _displayLogin, ExecuteDisplayLogin, CanExecuteDisplayLogin ) ) ;
        
            window.CommandBindings.Add(
                new CommandBinding(_history, ExecuteDisplayHistory, CanExecuteDisplayHistory));

            window.CommandBindings.Add(
                new CommandBinding(_displayMucMarks, ExecuteDisplayMucMarks, CanExecuteDisplayMucMarks));
        }

	    private static void CanExecuteDisplayMucMarks(object sender, CanExecuteRoutedEventArgs e)
	    {
            e.CanExecute = (MucMarks.Instance.Count > 0);
            e.Handled = true;
        }

	    private static void ExecuteDisplayMucMarks(object sender, ExecutedRoutedEventArgs e)
	    {
            e.Handled = false;
	    }

	    private static void CanExecuteDisplayHistory(object sender, CanExecuteRoutedEventArgs e)
	    {
	        e.CanExecute = (RecentItems.Instance.Count > 0);
	        e.Handled = true;
	    }

	    private static void ExecuteDisplayHistory(object sender, ExecutedRoutedEventArgs e)
	    {
	        e.Handled = false;
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