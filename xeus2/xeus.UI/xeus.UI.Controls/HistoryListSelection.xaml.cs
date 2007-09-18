using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using xeus2.xeus.Core;
using xeus2.xeus.Middle;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    /// <summary>
    /// Interaction logic for HistoryListSelection.xaml
    /// </summary>
    public partial class HistoryListSelection : UserControl
    {
        public HistoryListSelection()
        {
            InitializeComponent();
        }

        private void _list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;

            if (e.AddedItems.Count > 0)
            {
                Recent recent = (Recent) e.AddedItems[0];

                if (recent.Item is IContact)
                {
                    Middle.Chat.Instance.DisplayChat((IContact)recent.Item);
                }
                else
                {
                    MucInfo.Instance.MucLogin((Service)recent.Item, null);
                }

                _list.SelectedItem = null;

                CloseParentPopup();
            }
        }

        private void CloseParentPopup()
        {
            Popup popup = Parent as Popup;

            if (popup != null)
            {
                popup.IsOpen = false;
            }
        }

    }
}
