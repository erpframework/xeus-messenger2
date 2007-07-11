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

    public partial class ChangeNickInRoom : System.Windows.Window
    {
        public ChangeNickInRoom()
        {
            InitializeComponent();
        }

        public string Nick
        {
            get
            {
                return _nick.Text;
            }
        }

        protected void OnChange(object sender, RoutedEventArgs eventArgs)
        {
            DialogResult = true;
            Close();
        }
    }
}