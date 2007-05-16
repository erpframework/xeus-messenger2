using System.Windows.Controls ;

namespace xeus2.xeus.XData
{
	internal class XDataCheckBox : XDataControl
	{
		private CheckBox _checkBox = new CheckBox() ;

		protected override void OnFieldIsSet()
		{
			base.OnFieldIsSet() ;

			_description.Visibility = System.Windows.Visibility.Collapsed ;

			_container.Children.Add( _checkBox ) ;

			_checkBox.IsChecked = Field.GetValueBool() ;
			_checkBox.Content = Field.Label ;
			_checkBox.ToolTip = Field.Description ;
		}
	}
}