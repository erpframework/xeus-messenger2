using System.Windows ;
using System.Windows.Controls;
using xeus2.Properties ;
using xeus2.xeus.Commands ;
using xeus2.xeus.Core ;

namespace xeus2.xeus.UI
{
	/// <summary>
	/// Interaction logic for Muc.xaml
	/// </summary>
	public partial class Muc : Window
	{
		private MucRoom _mucRoom ;

		internal Muc( Service service, string nick, string password )
		{
			InitializeComponent() ;

			_mucRoom = Account.Instance.JoinMuc( service, nick, password ) ;

            _mucRoom.OnClickMucContact += new MucRoom.MucContactHandler(_mucRoom_OnClickMucContact);
            _mucRoom.MucMessages.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(MucMessages_CollectionChanged);

			DataContext = _mucRoom ;

			MucCommands.RegisterCommands( this );
		}

	    private ScrollViewer _scrollViewer = null;

        void MucMessages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (_scrollViewer == null)
            {
                _scrollViewer = (ScrollViewer) _flowViewer.Template.FindName("PART_ContentHost", _flowViewer);
            }

            if (_scrollViewer.VerticalOffset >= _scrollViewer.ScrollableHeight - 2.0)
            {
                _scrollViewer.ScrollToBottom();
            }
        }

        void _mucRoom_OnClickMucContact(MucMessage mucMessage)
        {
            if (!string.IsNullOrEmpty(mucMessage.Sender))
            {
                _text.Text = mucMessage.Sender + ": ";
                _text.Focus();
                _text.CaretIndex = _text.Text.Length;
            }
        }

		protected void OnSendMessage( object sender, RoutedEventArgs eventArgs )
		{
			_mucRoom.SendMessage( _text.Text ) ;
		    _text.Text = string.Empty;
		}

		protected override void OnClosing( System.ComponentModel.CancelEventArgs e )
		{
			base.OnClosing( e );

			_mucRoom.LeaveRoom( Settings.Default.MucLeaveMsg );
		}
	}
}
