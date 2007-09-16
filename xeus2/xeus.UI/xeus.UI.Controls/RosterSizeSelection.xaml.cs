using System;
using System.Windows.Controls;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    /// <summary>
    /// Interaction logic for RosterSizeSelection.xaml
    /// </summary>
    public partial class RosterSizeSelection : UserControl
    {
        public RosterSizeSelection()
        {
            InitializeComponent();

            Loaded += RosterSizeSelection_Loaded;
        }

        void RosterSizeSelection_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            DummyContact dummyContact = new DummyContact();

            lock (Roster.Instance.Items._syncObject)
            {
                int count = Roster.Instance.Items.Count;

                IContact contact = dummyContact;

                if (count > 0)
                {
                    Random random = new Random();

                    for (int i = 0; i < 20; i++)
                    {
                        contact = Roster.Instance.Items[random.Next(0, count - 1)];

                        if (contact.IsAvailable)
                        {
                            break;
                        }
                    }
                }

                _small.Content = _medium.Content = _big.Content = contact;
            }
        }

        private void CloseParentPopup()
        {
            System.Windows.Controls.Primitives.Popup popup = Parent as System.Windows.Controls.Primitives.Popup;

            if (popup != null)
            {
                popup.IsOpen = false;
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CloseParentPopup();
        }
    }
}