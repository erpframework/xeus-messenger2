using System.Windows ;
using System.Windows.Controls ;
using agsXMPP.protocol.x.data ;

namespace xeus2.xeus.XData
{
	internal class XDataCheckBox : XDataControl
	{
		private CheckBox _checkBox = new CheckBox() ;

		protected override void OnFieldIsSet()
		{
			base.OnFieldIsSet() ;

			_description.Visibility = Visibility.Collapsed ;

			_container.Children.Add( _checkBox ) ;

			_checkBox.IsChecked = Field.GetValueBool() ;
			_checkBox.Content = Field.Label ;
			_checkBox.ToolTip = Field.Description ;
		}

		public override Field GetResult()
		{
			Field.SetValueBool( ( bool ) _checkBox.IsChecked ) ;

			return Field ;
		}
	}
}