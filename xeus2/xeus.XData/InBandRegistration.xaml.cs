using System.Windows ;
using System.Windows.Controls ;
using agsXMPP ;
using agsXMPP.protocol.iq.register ;
using agsXMPP.protocol.x.data ;
using agsXMPP.Xml.Dom ;
using xeus2.xeus.Core ;

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
		private XDataContainer _xDataContainer = null ;
		private Service _service = null ;

		private Data _xData = null ;

		internal void Setup( Register register, Service service )
		{
			_register = register ;
			_service = service ;

			if ( _register.HasChildElements )
			{
				foreach ( Node node in _register.ChildNodes )
				{
					if ( node is Data )
					{
						_xData = node as Data ;
						break ;
					}
				}
			}

			if ( _xData == null )
			{
				SetupGatewayRegistration() ;
			}
			else
			{
				SetupXDataRegistration( _xData ) ;
			}
		}

		public Register Register
		{
			get
			{
				return _register ;
			}
		}

		private void SetupXDataRegistration( Data xData )
		{
			if ( string.IsNullOrEmpty( xData.Title ) )
			{
				_title.Visibility = Visibility.Collapsed ;
			}
			else
			{
				_title.Text = xData.Title ;
			}

			if ( string.IsNullOrEmpty( xData.Instructions ) )
			{
				_instructions.Visibility = Visibility.Collapsed ;
			}
			else
			{
				_instructions.Text = xData.Instructions ;
			}

			_xDataContainer = new XDataContainer() ;
			_container.Children.Add( _xDataContainer ) ;

			_xDataContainer.Data = xData ;
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

		public Data GetResult()
		{
			if ( _xDataContainer != null )
			{
				return _xDataContainer.GetResult() ;
			}
			else
			{
				return null ;
			}
		}

		public Data XData
		{
			get
			{
				if ( _xDataContainer != null )
				{
					return _xDataContainer.Data ;
				}
				else
				{
					return null ;
				}
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

		internal Service Service
		{
			get
			{
				return _service ;
			}
		}
	}
}