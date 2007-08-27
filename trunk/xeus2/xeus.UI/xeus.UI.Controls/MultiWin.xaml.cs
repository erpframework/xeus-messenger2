using System.Windows;
using System.Windows.Controls;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    /// <summary>
    /// Interaction logic for MultiWin.xaml
    /// </summary>
    public partial class MultiWin : UserControl
    {
        public MultiWin()
        {
            InitializeComponent();
        }

        public MultiWin(UIElement element):this()
        {
            ContentElement = element;
        }

        public UIElement ContentElement
        {
            get
            {
                return _container.Child;
            }
            set
            {
                _container.Child = value;
            }
        }
    }
}