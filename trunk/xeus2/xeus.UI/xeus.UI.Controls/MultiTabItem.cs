using System.Windows;
using System.Windows.Controls;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    internal class MultiTabItem : NotifyInfoDispatcher
    {

        public string Name
        {
            get
            {
                return _name;
            }
        }

        private readonly string _name;
        private GridSplitter _gridSplitter = new GridSplitter();
        private ColumnDefinition _columnDefinition = new ColumnDefinition();

        public MultiWin Container
        {
            get
            {
                return _container;
            }
        }

        public bool IsVisible
        {
            get
            {
                return (_container.Visibility == Visibility.Visible);
            }
            set
            {
                _container.Visibility = (value) ? Visibility.Visible : Visibility.Collapsed;

                NotifyPropertyChanged("IsVisible");
            }
        }

        public GridSplitter GridSplitter
        {
            get
            {
                return _gridSplitter;
            }
        }

        public ColumnDefinition ColumnDefinition
        {
            get
            {
                return _columnDefinition;
            }
        }

        private readonly MultiWin _container;

        internal MultiTabItem(string name, MultiWin container)
        {
            _name = name;
            _container = container;

            _gridSplitter.Width = 5;
            _gridSplitter.ResizeDirection = GridResizeDirection.Auto;
            _gridSplitter.ResizeBehavior = GridResizeBehavior.BasedOnAlignment;
        }
    }
}