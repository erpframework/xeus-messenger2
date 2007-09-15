using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Threading;

namespace xeus2.xeus.Core
{
    public class ObservableCollectionDisp<T> : ObservableCollection<T>
    {
        public readonly object _syncObject = new object();
        private bool _dontSendChange = false;

        private delegate void SetItemCallback(int index, T item);

        private delegate void RemoveItemCallback(int index);

        private delegate void ClearItemsCallback();

        private delegate void InsertItemCallback(int index, T item);

        private delegate void MoveItemCallback(int oldIndex, int newIndex);

        private delegate void AddCallback(IList<T> items);

        public const DispatcherPriority _dispatcherPriority = DispatcherPriority.Render ;

        protected override void SetItem(int index, T item)
        {
            if (App.CheckAccessSafe())
            {
                lock (_syncObject)
                {
                    base.SetItem(index, item);
                }
            }
            else
            {
                if (App.Current != null)
                {
                    App.Current.Dispatcher.BeginInvoke(_dispatcherPriority,
                                    new SetItemCallback(SetItem), index, new object[] {item});
                }
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_dontSendChange)
            {
                base.OnCollectionChanged(e);
            }
        }

        public void Add(IList<T> items)
        {
            if (App.CheckAccessSafe())
            {
                _dontSendChange = true;

                // required notification parameter
                ArrayList list = new ArrayList();

                lock (_syncObject)
                {
                    foreach (T item in items)
                    {
                        base.InsertItem(Count, item);
                        list.Add(item);
                    }
                }

                _dontSendChange = false;


                NotifyCollectionChangedEventArgs e =
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list);

                OnCollectionChanged(e);
            }
            else
            {
                if (App.Current != null)
                {
                    App.Current.Dispatcher.BeginInvoke(_dispatcherPriority, new AddCallback(Add), items);
                }
            }
        }

        protected override void InsertItem(int index, T item)
        {
            if (App.CheckAccessSafe())
            {
                lock (_syncObject)
                {
                    if (index > Count)
                    {
                        base.InsertItem(Count, item);
                    }
                    else
                    {
                        base.InsertItem(index, item);
                    }
                }
            }
            else
            {
                if (App.Current != null)
                {
                    App.Current.Dispatcher.BeginInvoke(_dispatcherPriority, new InsertItemCallback(InsertItem), index, new object[] {item});
                }
            }
        }


        protected override void RemoveItem(int index)
        {
            if (App.CheckAccessSafe())
            {
                lock (_syncObject)
                {
                    base.RemoveItem(index);
                }
            }
            else
            {
                if (App.Current != null)
                {
                    App.Current.Dispatcher.BeginInvoke(_dispatcherPriority,
                                                       new RemoveItemCallback(RemoveItem), index, new object[] {});
                }
            }
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            if (App.CheckAccessSafe())
            {
                lock (_syncObject)
                {
                    base.MoveItem(oldIndex, newIndex);
                }
            }
            else
            {
                if (App.Current != null)
                {
                    App.Current.Dispatcher.BeginInvoke(_dispatcherPriority,
                                    new MoveItemCallback(MoveItem), oldIndex, new object[] {newIndex});
                }
            }
        }

        protected override void ClearItems()
        {
            if (App.CheckAccessSafe())
            {
                lock (_syncObject)
                {
                    base.ClearItems();
                }
            }
            else
            {
                if (App.Current != null)
                {
                    App.Current.Dispatcher.BeginInvoke(_dispatcherPriority, new ClearItemsCallback(ClearItems));
                }
            }
        }
    }
}