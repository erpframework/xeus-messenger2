using System.Windows ;
using agsXMPP ;
using agsXMPP.protocol.iq.register ;
using xeus2.xeus.Core ;

namespace xeus2.xeus.UI
{
	/// <summary>
	/// Interaction logic for Registration.xaml
	/// </summary>
	public partial class Registration : Window
	{
		public Registration( Register register, Jid from )
		{
			InitializeComponent() ;

			_registration.Setup( register, from ) ;
		}

		protected void OnRegister( object sender, RoutedEventArgs eventArgs )
		{
			_registration.UpdateData();

			Account.Instance.DoRegisterService( _registration.Jid, _registration.UserName,
												_registration.Password, _registration.Email );
		}
	}
}