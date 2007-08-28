using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    /// <summary>
    /// Interaction logic for MultiTabControl.xaml
    /// </summary>
    public partial class MultiTabControl : UserControl
    {
        private ObservableCollectionDisp<MultiTabItem> _multiWindows = new ObservableCollectionDisp<MultiTabItem>();

        private IMultiWinContainerProvider _multiWinContainerProvider = null;

        public MultiTabControl()
        {
            InitializeComponent();

            _multiWindows.CollectionChanged += new NotifyCollectionChangedEventHandler(_multiWindows_CollectionChanged);

            _tabs.DataContext = MultiWindows;
        }

        private void _multiWindows_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (MultiTabItem multiWin in e.OldItems)
                        {
                            _container.Children.Remove(multiWin.Container);
                            _container.Children.Remove(multiWin.GridSplitter);

                            multiWin.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(multiWin_PropertyChanged);
                            multiWin.Container.OnMultiWinEvent -= new MultiWin.NotifyMultiWin(Container_OnMultiWinEvent);

                            RedistributeColumns();
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Replace:
                    {
                        foreach (MultiTabItem multiWin in e.NewItems)
                        {
                            multiWin.Container.OnMultiWinEvent += new MultiWin.NotifyMultiWin(Container_OnMultiWinEvent);
                            multiWin.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(multiWin_PropertyChanged);

                            _container.Children.Add(multiWin.Container);
                            _container.Children.Add(multiWin.GridSplitter);

                            RedistributeColumns();
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Reset:
                    {
                        _container.Children.Clear();

                        break;
                    }
            }
        }

        void Container_OnMultiWinEvent(MultiWin sender, MultiWin.MultiWinEvent multiWinEvent)
        {
            MultiTabItem tabItem = null;

            lock (_multiWindows._syncObject)
            {
                foreach (MultiTabItem multiTabItem in _multiWindows)
                {
                    if (multiTabItem.Container == sender)
                    {
                        tabItem = multiTabItem;
                        break;
                    }
                }               
            }

            if (tabItem != null)
            {
                switch (multiWinEvent)
                {
                    case MultiWin.MultiWinEvent.Close:
                        {
                            _multiWindows.Remove(tabItem);
                            break;
                        }
                    case MultiWin.MultiWinEvent.Hide:
                        {
                            tabItem.IsVisible = false;
                            break;
                        }
                    case MultiWin.MultiWinEvent.Flyout:
                        {
                            _multiWindows.Remove(tabItem);

                            MultiWinFlyout flyout = new MultiWinFlyout(_multiWinContainerProvider, tabItem.Container, tabItem.Name);
                            flyout.Show();
                            break;
                        }
                }
            }
        }

        void multiWin_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsVisible")
            {
                RedistributeColumns();
            }
        }

        void RedistributeColumns()
        {
            List <MultiTabItem> activeItems = new List<MultiTabItem>();

            lock (_multiWindows._syncObject)
            {
                foreach (MultiTabItem multiTabItem in _multiWindows)
                {
                    if (multiTabItem.IsVisible)
                    {
                        activeItems.Add(multiTabItem);
                    }
                }
            }

            _container.ColumnDefinitions.Clear();

            foreach (MultiTabItem multiTabItem in activeItems)
            {
                _container.ColumnDefinitions.Add(multiTabItem.ColumnDefinition);
                Grid.SetColumn(multiTabItem.Container, _container.ColumnDefinitions.Count - 1);
                Grid.SetColumn(multiTabItem.GridSplitter, _container.ColumnDefinitions.Count - 1);

                multiTabItem.IsLast = false;
                multiTabItem.Container.DisplayControls = true;
            }

            // hide last splitter
            if (_container.ColumnDefinitions.Count > 0)
            {
                activeItems[(_container.ColumnDefinitions.Count - 1)].IsLast = true;
            }

            // hide controls
            if (_multiWindows.Count <= 1)
            {
                activeItems[(_container.ColumnDefinitions.Count - 1)].Container.DisplayControls = false;
            }

            // hide Hide button
            if (activeItems.Count <= 1)
            {
                activeItems[(_container.ColumnDefinitions.Count - 1)].Container.DisplayControls = false;
            }

            // hide tabs
            _tabs.Visibility = (_multiWindows.Count <= 1)
                                   ? System.Windows.Visibility.Collapsed
                                   : System.Windows.Visibility.Visible;
        }

        internal ObservableCollectionDisp<MultiTabItem> MultiWindows
        {
            get
            {
                return _multiWindows;
            }
        }

        internal IMultiWinContainerProvider MultiWinContainerProvider
        {
            get
            {
                return _multiWinContainerProvider;
            }
            set
            {
                _multiWinContainerProvider = value;
            }
        }

        void OnUnload(object sender, RoutedEventArgs args)
        {
            _multiWindows.Clear();
        }
    }
}