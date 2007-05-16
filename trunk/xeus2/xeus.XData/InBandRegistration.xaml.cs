using System.Windows ;
using System.Windows.Controls ;
using agsXMPP.protocol.iq.register ;
using agsXMPP.protocol.x.data ;
using agsXMPP.Xml.Dom ;

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

		public Register Register
		{
			get
			{
				return _register ;
			}

			set
			{
				_register = value ;

				Data xData = null ;

				if ( _register.HasChildElements )
				{
					foreach ( Node node in _register.ChildNodes )
					{
						if ( node is Data )
						{
							xData = node as Data ;
							break ;
						}
					}
				}

				if ( xData == null )
				{
					SetupGatewayRegistration() ;
				}
				else
				{
					SetupXDataRegistration( xData ) ;
				}
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

		private void SetupGatewayRegistration()
		{
			_instructions.Text = _register.Instructions ;

			// user name
			if ( _register.Username != null )
			{
				XDataTextBox textUserName = new XDataTextBox() ;

				Field fieldUserName = new Field( "username", Properties.Resources.Constant_UserName, FieldType.Text_Single ) ;
				fieldUserName.IsRequired = true ;
				fieldUserName.AddValue( _register.Username ) ;
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
				password.AddValue( _register.Password ) ;
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
				fieldEmail.AddValue( _register.Email ) ;
				fieldEmail.Description = Properties.Resources.Constant_EnterEmailForService ;

				email.Field = fieldEmail ;

				_container.Children.Add( email ) ;
			}
		}
	}
}