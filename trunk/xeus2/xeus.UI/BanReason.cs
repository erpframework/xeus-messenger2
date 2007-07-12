using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace xeus2.xeus.UI
{
    /// <summary>
    /// Interaction logic for RoomTopic.xaml
    /// </summary>

    public partial class BanReason : System.Windows.Window
    {
        public BanReason()
        {
            InitializeComponent();
        }

        public string Reason
        {
            get
            {
                return _reason.Text;
            }
        }

        protected void OnBan(object sender, RoutedEventArgs eventArgs)
        {
            DialogResult = true;
            Close();
        }
    }
}