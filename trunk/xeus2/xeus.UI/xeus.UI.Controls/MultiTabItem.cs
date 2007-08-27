using System.Windows;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    internal class MultiTabItem
    {

        public string Name
        {
            get
            {
                return _name;
            }
        }

        private readonly string _name;

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
            }
        }

        private readonly MultiWin _container;

        internal MultiTabItem(string name, MultiWin container)
        {
            _name = name;
            _container = container;
        }
    }
}