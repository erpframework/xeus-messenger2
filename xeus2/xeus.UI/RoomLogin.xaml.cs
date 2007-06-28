using System.Windows ;
using xeus2.xeus.Core ;

namespace xeus2.xeus.UI
{
	/// <summary>
	/// Interaction logic for RoomLogin.xaml
	/// </summary>
	public partial class RoomLogin : Window
	{
		internal RoomLogin( Service service, string forceNick )
		{
			InitializeComponent() ;

			if ( !string.IsNullOrEmpty( forceNick ) )
			{
				_nick.Text = forceNick ;
				_nick.IsEnabled = false ;
			}
            else
			{
			    _nick.Text = Account.Instance.MyJid.User;
			}

			if ( !service.IsMucPasswordProtected )
			{
				_passwordPanel.Visibility = Visibility.Collapsed ;
			}
		}

		protected void OnJoin( object sender, RoutedEventArgs eventArgs )
		{
			Middle.Muc.Instance.DisplayMuc( DataContext as Service, _nick.Text, _password.Password );
			Close() ;
		}
	}
}