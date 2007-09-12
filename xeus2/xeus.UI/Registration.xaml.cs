using System.Windows ;
using agsXMPP.protocol.iq.register ;
using xeus2.xeus.Core ;

namespace xeus2.xeus.UI
{
	/// <summary>
	/// Interaction logic for Registration.xaml
	/// </summary>
	public partial class Registration : BaseWindow
	{
	    public const string _keyBase = "Registration";

		internal Registration( Register register, Service service ) : base(_keyBase, service.Jid.Bare)
		{
			InitializeComponent() ;

            DataContext = service;

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
				Account.Instance.DoRegisterService( _registration.Service, _registration.GetValues() ) ;
			}

			Close() ;
		}
	}
}