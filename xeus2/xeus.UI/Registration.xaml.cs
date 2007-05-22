using System.Windows ;
using agsXMPP ;
using agsXMPP.protocol.iq.register ;
using xeus2.xeus.Commands ;
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
			if ( !_registration.IsValid )
			{
				return ;
			}

			if ( _registration.XData != null )
			{
				Account.Instance.DoRegisterService( _registration.Service, _registration.GetResult() ) ;
			}
			else
			{
				_registration.UpdateData();
	
				Account.Instance.DoRegisterService( _registration.Service, _registration.UserName,
				                                    _registration.Password, _registration.Email ) ;
			}

			Close() ;
		}
	}
}