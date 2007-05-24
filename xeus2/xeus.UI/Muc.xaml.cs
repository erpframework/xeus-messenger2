using System.Windows ;
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
		}
	}
}