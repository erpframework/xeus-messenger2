using System.Collections.Generic ;
using System.Windows ;

namespace xeus2.xeus.Middle
{
	internal class WindowManager< K, T > where T : Window
	{
		private Dictionary< K, T > _windows = new Dictionary< K, T >() ;

		private object _lock = new object() ;

		protected void AddWindow( K key, T window )
		{
			lock ( _lock )
			{
				_windows.Add( key, window ) ;
			}
		}

		protected void RemoveWindow( K key )
		{
			lock ( _lock )
			{
				_windows.Remove( key ) ;
			}
		}

		protected T GetWindow( K key )
		{
			lock ( _lock )
			{
				if ( !_windows.ContainsKey( key ) )
				{
					return null ;
				}

				return _windows[ key ] ;
			}
		}
	}
}