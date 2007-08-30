using System ;
using System.ComponentModel ;
using System.Windows.Threading ;

namespace xeus2.xeus.Core
{
	public class NotifyInfoDispatcher : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged ;

		public delegate void NotifyPropertyChangedHandler( String info ) ;

		protected void NotifyPropertyChanged( String info )
		{
			if ( PropertyChanged != null )
			{
				if ( App.CheckAccessSafe() )
				{
					PropertyChanged( this, new PropertyChangedEventArgs( info ) ) ;
				}
				else
				{
					App.InvokeSafe( DispatcherPriority.Background,
					                new NotifyPropertyChangedHandler( NotifyPropertyChanged ), info ) ;
				}
			}
		}
	}
}