using System.Windows.Controls ;
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

		public override agsXMPP.protocol.x.data.Field GetResult()
		{
			Field.SetValue( _password.Password ) ;

			return Field ;
		}
	}
}