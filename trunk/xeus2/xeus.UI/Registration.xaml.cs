using System.Windows ;
using agsXMPP.protocol.iq.register ;

namespace xeus2.xeus.UI
{
	/// <summary>
	/// Interaction logic for Registration.xaml
	/// </summary>
	public partial class Registration : Window
	{
		public Registration( Register register )
		{
			InitializeComponent() ;

			_registration.Register = register ;
		}
	}
}