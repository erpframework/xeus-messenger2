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
		internal Registration( Register register, Service service )
		{
			InitializeComponent() ;

			_registration.Setup( register, service ) ;
		}

		protected void OnRegister( object sender, RoutedEventArgs eventArgs )
		{
			_registration.UpdateData();

			if ( _registration.XData != null )
			{
				Account.Instance.DoRegisterService( _registration.Service, _registration.XData ) ;
			}
			else
			{
				Account.Instance.DoRegisterService( _registration.Service, _registration.UserName,
				                                    _registration.Password, _registration.Email ) ;
			}
		}
	}
}