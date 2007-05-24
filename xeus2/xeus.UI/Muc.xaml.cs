using System.Windows ;
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

			DataContext = _mucRoom ;

			MucCommands.RegisterCommands( this );
		}

		protected void OnSendMessage( object sender, RoutedEventArgs eventArgs )
		{
			_mucRoom.SendMessage( _text.Text ) ;
		}

		protected override void OnClosing( System.ComponentModel.CancelEventArgs e )
		{
			base.OnClosing( e );

			_mucRoom.LeaveRoom( Settings.Default.MucLeaveMsg );
		}
	}
}