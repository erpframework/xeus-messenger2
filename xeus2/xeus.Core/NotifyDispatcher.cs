using System ;
using System.ComponentModel ;
using System.Windows.Threading ;

namespace xeus2.xeus.Core
{
	internal class NotifyInfoDispatcher : INotifyPropertyChanged
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
					App.InvokeSafe( DispatcherPriority.Send,
					                new NotifyPropertyChangedHandler( NotifyPropertyChanged ), info ) ;
				}
			}
		}
	}
}