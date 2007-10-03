using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    /// <summary>
    /// Interaction logic for UserSearchText.xaml
    /// </summary>
    public partial class UserSearchText : UserControl
    {
        public UserSearchText()
        {
            InitializeComponent();

            _textFilter.PreviewKeyDown += _textFilter_PreviewKeyDown;
        }

        private void _textFilter_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    {
                        _textFilter.Clear();
                        break;
                    }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _textFilter.Clear();
        }
    }
}