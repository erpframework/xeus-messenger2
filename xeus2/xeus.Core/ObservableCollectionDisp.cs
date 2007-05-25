using System.Collections.ObjectModel ;
using System.Windows.Threading ;

namespace xeus2.xeus.Core
{
	public class ObservableCollectionDisp< T > : ObservableCollection< T >
	{
		public readonly object _syncObject = new object() ;

		private delegate void SetItemCallback( int index, T item ) ;

		private delegate void RemoveItemCallback( int index ) ;

		private delegate void ClearItemsCallback() ;

		private delegate void InsertItemCallback( int index, T item ) ;

		private delegate void MoveItemCallback( int oldIndex, int newIndex ) ;

		protected override void SetItem( int index, T item )
		{
			if ( App.CheckAccessSafe() )
			{
				lock ( _syncObject )
				{
					base.SetItem( index, item ) ;
				}
			}
			else
			{
				if ( App.Current != null )
				{
					App.Current.Dispatcher.BeginInvoke( DispatcherPriority.Send,
					                                    new SetItemCallback( SetItem ), index, new object[] { item } ) ;
				}
			}
		}

		protected override void InsertItem( int index, T item )
		{
			if ( App.CheckAccessSafe() )
			{
				lock ( _syncObject )
				{
					if ( index > Count )
					{
						base.InsertItem( Count, item ) ;
					}
					else
					{
						base.InsertItem( index, item ) ;
					}
				}
			}
			else
			{
				if ( App.Current != null )
				{
					App.Current.Dispatcher.BeginInvoke( DispatcherPriority.Send,
					                                    new InsertItemCallback( InsertItem ), index, new object[] { item } ) ;
				}
			}
		}


		protected override void RemoveItem( int index )
		{
			if ( App.CheckAccessSafe() )
			{
				lock ( _syncObject )
				{
					base.RemoveItem( index ) ;
				}
			}
			else
			{
				if ( App.Current != null )
				{
					App.Current.Dispatcher.BeginInvoke( DispatcherPriority.Send,
					                                    new RemoveItemCallback( RemoveItem ), index, new object[] { } ) ;
				}
			}
		}

		protected override void MoveItem( int oldIndex, int newIndex )
		{
			if ( App.CheckAccessSafe() )
			{
				lock ( _syncObject )
				{
					base.MoveItem( oldIndex, newIndex ) ;
				}
			}
			else
			{
				if ( App.Current != null )
				{
					App.Current.Dispatcher.BeginInvoke( DispatcherPriority.Send,
					                                    new MoveItemCallback( MoveItem ), oldIndex, new object[] { newIndex } ) ;
				}
			}
		}

		protected override void ClearItems()
		{
			if ( App.CheckAccessSafe() )
			{
				lock ( _syncObject )
				{
					base.ClearItems() ;
				}
			}
			else
			{
				if ( App.Current != null )
				{
					App.Current.Dispatcher.BeginInvoke( DispatcherPriority.Send, new ClearItemsCallback( ClearItems ) ) ;
				}
			}
		}
	}
}