using System.Windows.Controls ;
using agsXMPP.protocol.x.data ;
using xeus2.xeus.UI ;

namespace xeus2.xeus.XData
{
	internal class XDataSecret : XDataControl
	{
		private PasswordBox _password = new PasswordBox() ;

		protected override void OnInitialized( System.EventArgs e )
		{
			base.OnInitialized( e );

			_password.Style = StyleManager.GetStyle( "XDataSecret" ) ;
		}

		protected override void OnFieldIsSet()
		{
			base.OnFieldIsSet() ;

			_container.Children.Add( _password ) ;

			foreach ( string text in Field.GetValues() )
			{
				_password.Password = text ;
			}
		}

		public override Field GetResult()
		{
			Field field = new Field( Field.Var, null, Field.Type );
			field.SetValue( _password.Password ) ;

			return field ;
		}

		public override bool Validate()
		{
			return ( !Field.IsRequired || _password.Password.Length > 0 ) ;
		}
	}
}