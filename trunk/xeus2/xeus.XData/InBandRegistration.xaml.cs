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

				_instructions.Text = _register.Instructions ;

				// user name
				if ( _register.Username != null )
				{
					XDataTextBox textUserName = new XDataTextBox() ;

					Field fieldUserName = new Field( "username", Properties.Resources.Constant_UserName, FieldType.Text_Single ) ;
					fieldUserName.Value = _register.Username ;
					fieldUserName.Description = Properties.Resources.Constant_EnterLoginNameForService ;

					textUserName.Field = fieldUserName ;

					_container.Children.Add( textUserName ) ;
				}

				if ( _register.Password != null )
				{
					XDataSecret xDataSecret = new XDataSecret();

					Field password = new Field( "password", "Password", FieldType.Text_Private ) ;
					password.Value = _register.Username ;
					password.Description = "Enter your password for this Service" ;

					xDataSecret.Field = password ;

					_container.Children.Add( xDataSecret ) ;
				}

				// password
			}
		}
	}
}