using xeus2.xeus.Commands;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI
{
    /// <summary>
    /// Interaction logic for FileTransferWindow.xaml
    /// </summary>
    public partial class FileTransferWindow : BaseWindow
    {
        public const string _keyBase = "FileTransfer";

        internal FileTransferWindow() : base(_keyBase, string.Empty)
        {
            InitializeComponent();

            GeneralCommands.RegisterCommands(this);
        }
    }
}
