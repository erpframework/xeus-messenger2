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
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Replace:
                    {
                        foreach (MultiTabItem multiWin in e.NewItems)
                        {
                            _container.Children.Add(multiWin.Container);
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

        internal ObservableCollectionDisp<MultiTabItem> MultiWindows
        {
            get
            {
                return _multiWindows;
            }
        }
    }
}