using System.Windows.Controls ;

namespace xeus2.xeus.XData
{
	internal class XDataSecret : XDataControl
	{
		private PasswordBox _password = new PasswordBox() ;

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