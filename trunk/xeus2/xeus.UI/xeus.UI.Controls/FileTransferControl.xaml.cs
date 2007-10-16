using System.Windows.Controls;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    /// <summary>
    /// Interaction logic for FileTransferControl.xaml
    /// </summary>
    public partial class FileTransferControl : UserControl
    {
        private FileTransfer _fileTransfer;

        public FileTransferControl()
        {
            InitializeComponent();
        }

        internal FileTransfer FileTransfer
        {
            get
            {
                return _fileTransfer;
            }
            set
            {
                _fileTransfer = value;
            }
        }

        private void Button_Click_Accept(object sender, System.Windows.RoutedEventArgs e)
        {
            _fileTransfer.Accept();
        }

        private void Button_Click_Refuse(object sender, System.Windows.RoutedEventArgs e)
        {
            _fileTransfer.Refuse();
        }
    }
}