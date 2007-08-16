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
using xeus2.xeus.Core;

namespace xeus2.xeus.UI
{
    /// <summary>
    /// Interaction logic for MucOptions.xaml
    /// </summary>

    public partial class MucOptions : System.Windows.Window
    {
        private readonly MucRoom _mucRoom;

        internal MucOptions(MucRoom mucRoom)
        {
            _mucRoom = mucRoom;

            InitializeComponent();

            _muc.Setup(mucRoom);
        }

        void OnSaveConfig(object sender, RoutedEventArgs args)
        {
            _muc.Save();
        }
    }
}