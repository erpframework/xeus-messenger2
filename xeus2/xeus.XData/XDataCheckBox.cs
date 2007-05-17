using System.Windows ;
using System.Windows.Controls ;
using agsXMPP.protocol.x.data ;
using xeus2.xeus.UI ;

namespace xeus2.xeus.XData
{
	internal class XDataCheckBox : XDataControl
	{
		private CheckBox _checkBox = new CheckBox() ;

		protected override void OnInitialized( System.EventArgs e )
		{
			base.OnInitialized( e );

			_checkBox.Style = StyleManager.GetStyle( "XDataCheckBox" ) ;
		}

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
			Field field = new Field( Field.Var, null, Field.Type );
			field.SetValueBool( ( bool ) _checkBox.IsChecked ) ;

			return field ;
		}

		public override bool Validate()
		{
			return true ;
		}
	}
}