using System.Windows.Controls ;

namespace xeus2.xeus.XData
{
	internal class XDataCheckBox : XDataControl
	{
		private CheckBox _checkBox = new CheckBox() ;

		protected override void OnFieldIsSet()
		{
			base.OnFieldIsSet() ;

			_container.Children.Add( _checkBox ) ;

			_checkBox.IsChecked = Field.GetValueBool() ;
		}
	}
}