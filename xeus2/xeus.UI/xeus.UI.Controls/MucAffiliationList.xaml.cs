using System.Windows;
using System.Windows.Controls;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    /// <summary>
    /// Interaction logic for MucAffiliationList.xaml
    /// </summary>
    public partial class MucAffiliationList : UserControl
    {
        private MucAffContacts _affContacts = new MucAffContacts();

        public MucAffiliationList()
        {
            DataContext = _affContacts;

            InitializeComponent();
        }

        internal MucAffContacts AffContacts
        {
            get
            {
                return _affContacts;
            }
        }

        void Add(object sender, RoutedEventArgs args)
        {
            _affContacts.AddNew(_name.Text);
            _name.Text = string.Empty;
        }

        void Remove(object sender, RoutedEventArgs args)
        {
            _affContacts.Remove(_name.Text);
        }
    }
}