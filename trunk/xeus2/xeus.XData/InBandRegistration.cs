using System ;
using System.Windows ;
using System.Windows.Controls ;
using agsXMPP.protocol.iq.register ;
using agsXMPP.protocol.x.data ;
using agsXMPP.Xml.Dom ;
using xeus2.xeus.Commands ;
using xeus2.xeus.Core ;
using xeus2.xeus.Utilities ;

namespace xeus2.xeus.XData
{
	/// <summary>
	/// Interaction logic for InBandRegistration.xaml
	/// </summary>
	public class InBandRegistration : XDataFormBase
	{
		public InBandRegistration()
		{
			InitializeComponent() ;
		}

		private Register _register = null ;

		internal void Setup( Register register, Service service )
		{
			_register = register ;
			Service = service ;

			_xData = ElementUtil.GetData( _register ) ;

			if ( _xData == null )
			{
				SetupGatewayRegistration() ;
			}
			else
			{
				SetupXData( _xData ) ;
			}
		}

		public override bool IsValid
		{
			get
			{
				if ( XData == null )
				{
					if ( _register.Username != null
							&& _textUserName.GetResult().GetValue() == String.Empty )
					{
						return false ;
					}

					if ( _register.Password != null
							&& _textPassword.GetResult().GetValue() == String.Empty )
					{
						return false ;
					}

					if ( _register.Email != null
							&& _textEmail.GetResult().GetValue() == String.Empty )
					{
						return false ;
					}

					return true ;
				}
				else
				{
					return base.IsValid ;
				}
			}
		}

		public Register Register
		{
			get
			{
				return _register ;
			}
		}

		private XDataTextBox _textUserName ;
		private XDataSecret _textPassword ;
		private XDataTextBox _textEmail ;

		private void SetupGatewayRegistration()
		{
			_instructions.Text = _register.Instructions ;

			// user name
			if ( _register.Username != null )
			{
				_textUserName = new XDataTextBox() ;

				Field fieldUserName = new Field( "username", Properties.Resources.Constant_UserName, FieldType.Text_Single ) ;
				fieldUserName.IsRequired = true ;
				fieldUserName.AddValue( _register.Username ) ;
				fieldUserName.Description = Properties.Resources.Constant_EnterLoginNameForService ;

				_textUserName.Field = fieldUserName ;

				_container.Children.Add( _textUserName ) ;
			}

			// password
			if ( _register.Password != null )
			{
				_textPassword = new XDataSecret() ;

				Field password = new Field( "password", Properties.Resources.Constant_Password, FieldType.Text_Private ) ;
				password.IsRequired = true ;
				password.AddValue( _register.Password ) ;
				password.Description = Properties.Resources.Constant_EnterPasswordForService ;

				_textPassword.Field = password ;

				_container.Children.Add( _textPassword ) ;
			}

			// email
			if ( _register.Email != null )
			{
				_textEmail = new XDataTextBox() ;

				Field fieldEmail = new Field( "email", Properties.Resources.Constant_Email, FieldType.Text_Single ) ;
				fieldEmail.IsRequired = true ;
				fieldEmail.AddValue( _register.Email ) ;
				fieldEmail.Description = Properties.Resources.Constant_EnterEmailForService ;

				_textEmail.Field = fieldEmail ;

				_container.Children.Add( _textEmail ) ;
			}
		}

		public void UpdateData()
		{
			if ( _textUserName != null )
			{
				_register.Username = _textUserName.GetResult().GetValue() ;
			}

			if ( _textPassword != null )
			{
				_register.Password = _textPassword.GetResult().GetValue() ;
			}

			if ( _textEmail != null )
			{
				_register.Email = _textEmail.GetResult().GetValue() ;
			}
		}

		public string UserName
		{
			get
			{
				return _register.Username ;
			}
		}

		public string Password
		{
			get
			{
				return _register.Password ;
			}
		}

		public string Email
		{
			get
			{
				return _register.Email ;
			}
		}
	}
}