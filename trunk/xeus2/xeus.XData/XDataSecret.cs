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
			_password.Password = Field.Value ;
		}
	}
}