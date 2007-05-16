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

				SetupGatewayRegistration() ;
			}
		}

		private void SetupGatewayRegistration()
		{
			_instructions.Text = _register.Instructions ;

			// user name
			if ( _register.Username != null )
			{
				XDataTextBox textUserName = new XDataTextBox() ;

				Field fieldUserName = new Field( "username", Properties.Resources.Constant_UserName, FieldType.Text_Single ) ;
				fieldUserName.IsRequired = true ;
				fieldUserName.Value = _register.Username ;
				fieldUserName.Description = Properties.Resources.Constant_EnterLoginNameForService ;

				textUserName.Field = fieldUserName ;

				_container.Children.Add( textUserName ) ;
			}

			// password
			if ( _register.Password != null )
			{
				XDataSecret xDataSecret = new XDataSecret() ;

				Field password = new Field( "password", Properties.Resources.Constant_Password, FieldType.Text_Private ) ;
				password.IsRequired = true ;
				password.Value = _register.Username ;
				password.Description = Properties.Resources.Constant_EnterPasswordForService ;

				xDataSecret.Field = password ;

				_container.Children.Add( xDataSecret ) ;
			}

			// email
			if ( _register.Email != null )
			{
				XDataTextBox email = new XDataTextBox() ;

				Field fieldEmail = new Field( "email", Properties.Resources.Constant_Email, FieldType.Text_Single ) ;
				fieldEmail.IsRequired = true ;
				fieldEmail.Value = _register.Email ;
				fieldEmail.Description = Properties.Resources.Constant_EnterEmailForService ;

				email.Field = fieldEmail ;

				_container.Children.Add( email ) ;
			}
		}
	}
}