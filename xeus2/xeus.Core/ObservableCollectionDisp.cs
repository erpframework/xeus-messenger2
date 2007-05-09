using System ;
using System.Collections.ObjectModel ;
using System.Windows.Threading ;

namespace xeus2.xeus.Core
{
	public class ObservableCollectionDisp< T > : ObservableCollection< T >
	{
		public readonly object _syncObject = new object();

		private delegate void SetItemCallback( int index, T item ) ;

		private delegate void RemoveItemCallback( int index ) ;

		private delegate void ClearItemsCallback() ;

		private delegate void InsertItemCallback( int index, T item ) ;

		private delegate void MoveItemCallback( int oldIndex, int newIndex ) ;

		protected override void SetItem( int index, T item )
		{
			if ( App.Current.Dispatcher.CheckAccess() )
			{
				lock ( _syncObject )
				{
					base.SetItem( index, item ) ;
				}
			}
			else
			{
				App.Current.Dispatcher.BeginInvoke( DispatcherPriority.Send,
				                                new SetItemCallback( SetItem ), index, new object[] { item } ) ;
			}
		}

		protected override void InsertItem( int index, T item )
		{
			if ( App.Current.Dispatcher.CheckAccess() )
			{
				lock ( _syncObject )
				{
					if ( index > Count )
					{
						base.InsertItem( Count, item );
					}
					else
					{
						base.InsertItem( index, item );
					}
				}
			}
			else
			{
				App.Current.Dispatcher.BeginInvoke( DispatcherPriority.Send,
				                                new InsertItemCallback( InsertItem ), index, new object[] { item } ) ;
			}
		}


		protected override void RemoveItem( int index )
		{
			if ( App.Current.Dispatcher.CheckAccess() )
			{
				lock ( _syncObject )
				{
					base.RemoveItem( index ) ;
				}
			}
			else
			{
				App.Current.Dispatcher.BeginInvoke( DispatcherPriority.Send,
				                                new RemoveItemCallback( RemoveItem ), index, new object[] { } ) ;
			}
		}

		protected override void MoveItem( int oldIndex, int newIndex )
		{
			if ( App.Current.Dispatcher.CheckAccess() )
			{
				lock ( _syncObject )
				{
					base.MoveItem( oldIndex, newIndex ) ;
				}
			}
			else
			{
				App.Current.Dispatcher.BeginInvoke( DispatcherPriority.Send,
				                                new MoveItemCallback( MoveItem ), oldIndex, new object[] { newIndex } ) ;
			}
		}

		protected override void ClearItems()
		{
			if ( App.Current.Dispatcher.CheckAccess() )
			{
				lock ( _syncObject )
				{
					base.ClearItems() ;
				}
			}
			else
			{
				App.Current.Dispatcher.BeginInvoke( DispatcherPriority.Send, new ClearItemsCallback( ClearItems ) ) ;
			}
		}
	}
}