using System;
using System.Windows.Controls;

namespace xeus2.xeus.UI
{
    /// <summary>
    /// Interaction logic for SearchBox.xaml
    /// </summary>
    public partial class SearchBox : UserControl
    {
        public TextBox TextBox
        {
            get
            {
                return _text;
            }
        }
        public SearchBox()
        {
            InitializeComponent();
        }

        private void ClearClicked(object sender, EventArgs args)
        {
            _text.Text = String.Empty;
        }
    }
}