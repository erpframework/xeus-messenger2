using System.Windows.Controls ;
using agsXMPP.protocol.iq.register ;
using agsXMPP.protocol.x.data ;

namespace xeus2.xeus.XData
{
	/// <summary>
	/// Interaction logic for InBandRegistration.xaml
	/// </summary>
	public partial class InBandRegistration : UserControl
	{
		public InBandRegistration()
		{
			InitializeComponent() ;
		}

		private Register _register = null ;

		public Register Register
		{
			get
			{
				return _register ;
			}

			set
			{
				_register = value ;

				XDataTextBox textUserName = new XDataTextBox();

				Field fieldUserName = new Field( "username", _register.Username, FieldType.Text_Single );
				fieldUserName.Description = "User Name" ;

				textUserName.Field = fieldUserName ;
				
				_container.Children.Add( textUserName ) ;
			}
		}
	}
}