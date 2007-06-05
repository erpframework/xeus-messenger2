using System.Collections.Generic ;
using System.Windows ;
using agsXMPP.protocol.iq.register ;
using agsXMPP.protocol.x.data ;
using xeus2.xeus.Core ;
using xeus2.xeus.Utilities ;

namespace xeus2.xeus.XData
{
	/// <summary>
	/// Interaction logic for InBandRegistration.xaml
	/// </summary>
	public class InBandRegistration : XDataFormBase
	{
		private string[] _fields = new string[]
			{
				"username", "nick", "password",
				"name", "first", "last", "email", "address",
				"city", "state", "zip", "phone", "url", "date",
				"misc", "text", "key"
			} ;

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
				if ( _xData == null )
				{
					foreach ( UIElement element in _container.Children )
					{
						XDataControl control = element as XDataControl ;

						if ( control != null && !control.Validate() )
						{
							return false ;
						}
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

		private void AddField( string tag )
		{
			string value = _register.GetTag( tag ) ;

			if ( value != null )
			{
				XDataControl control ;

				if ( tag == "password" )
				{
					control = new XDataSecret() ;
				}
				else if ( tag == "key" )
				{
					control = new XDataHidden() ;
				}
				else
				{
					control = new XDataTextBox() ;
				}

				Field field = new Field( tag, TextUtil.ToTitleCase( tag ), FieldType.Text_Single ) ;
				field.IsRequired = false ;
				field.AddValue( value ) ;
				field.Description = tag ;

				control.Field = field ;

				_container.Children.Add( control ) ;
			}
		}

		public Dictionary< string, string > GetValues()
		{
			Dictionary< string, string > values = new Dictionary< string, string >() ;

			foreach ( UIElement element in _container.Children )
			{
				XDataControl control = element as XDataControl ;

				if ( control != null )
				{
					Field result = control.GetResult() ;
					values.Add( result.Var, result.GetValue() ) ;
				}
			}

			return values ;
		}

		private void SetupGatewayRegistration()
		{
			_instructions.Text = _register.Instructions ;

			if ( _register.GetTag( "registered" ) != null )
			{
				_instructions.Text += Properties.Resources.Constant_AlreadyRegistered ;
			}

			foreach ( string field in _fields )
			{
				AddField( field ) ;
			}
		}
	}
}