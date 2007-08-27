using System.Collections.Generic;
using System.Collections.Specialized;
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

                            RedistributeColumns();
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Replace:
                    {
                        foreach (MultiTabItem multiWin in e.NewItems)
                        {
                            multiWin.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(multiWin_PropertyChanged);

                            _container.Children.Add(multiWin.GridSplitter);
                            _container.Children.Add(multiWin.Container);
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
                Grid.SetColumn(multiTabItem.GridSplitter, _container.ColumnDefinitions.Count);
            }
        }

        internal ObservableCollectionDisp<MultiTabItem> MultiWindows
        {
            get
            {
                return _multiWindows;
            }
        }
    }
}