using System;
using System.Collections.Generic;
using System.ComponentModel ;
using System.Text;
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
				if ( App.Current.Dispatcher.CheckAccess() )
				{
					PropertyChanged( this, new PropertyChangedEventArgs( info ) ) ;
				}
				else
				{
					App.Current.Dispatcher.Invoke( DispatcherPriority.Send,
					                                  new NotifyPropertyChangedHandler( NotifyPropertyChanged ), info ) ;
				}
			}
		}
	}
}
