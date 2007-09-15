using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    /// <summary>
    /// Interaction logic for MucAffiliationList.xaml
    /// </summary>
    public partial class MucAffiliationList : UserControl
    {
        private MucAffContacts _affContacts = new MucAffContacts();

        private delegate void RefreshCallback();

        public MucAffiliationList()
        {
            DataContext = _affContacts;

            _affContacts.OnChange += new MucAffContacts.EventChangeCallback(_affContacts_OnChange);

            InitializeComponent();
        }

        private void _affContacts_OnChange(object sender, MucAffContact mucAffContact)
        {
            if (mucAffContact == null)
            {
                if (App.CheckAccessSafe())
                {
                    Refresh();
                }
                else
                {
                    if (App.Current != null)
                    {
                        App.InvokeSafe(App._dispatcherPriority, new RefreshCallback(Refresh));
                    }
                }
            }
        }

        private void Refresh()
        {
            View.Refresh();
        }

        internal MucAffContacts AffContacts
        {
            get
            {
                return _affContacts;
            }
        }

        internal ICollectionView View
        {
            get
            {
                return _list.ItemsSource as ListCollectionView;
            }
        }

        private void Add(object sender, RoutedEventArgs args)
        {
            _affContacts.AddNew(_name.Text);
            _name.Text = string.Empty;
        }
    }
}