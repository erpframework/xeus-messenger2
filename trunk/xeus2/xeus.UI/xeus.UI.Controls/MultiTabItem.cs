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

        private readonly ColumnDefinition _columnDefinition = new ColumnDefinition();
        private readonly GridSplitter _gridSplitter = new GridSplitter();

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

        public bool IsLast
        {
            get
            {
                return (GridSplitter.Visibility == Visibility.Collapsed);
            }

            set
            {
                if (value)
                {
                    GridSplitter.Visibility = Visibility.Collapsed;
                    Container.Margin = new Thickness(0, 0, 0, 0);
                }
                else
                {
                    GridSplitter.Visibility = Visibility.Visible;
                    Container.Margin = new Thickness(0, 0, 4.0, 0);
                }
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

            _gridSplitter.Width = 4;
            _gridSplitter.ResizeDirection = GridResizeDirection.Auto;
            _gridSplitter.ResizeBehavior = GridResizeBehavior.BasedOnAlignment;
        }
    }
}