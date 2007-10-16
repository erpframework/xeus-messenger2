using System.Windows.Controls;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    /// <summary>
    /// Interaction logic for FileTransferControl.xaml
    /// </summary>
    public partial class FileTransferControl : UserControl
    {
        public FileTransferControl()
        {
            InitializeComponent();

            _list.ItemsSource = FileTransfer.FileTransfers;
        }
    }
}