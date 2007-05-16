using System.Text ;
using System.Windows.Controls ;
using agsXMPP.protocol.x.data ;

namespace xeus2.xeus.XData
{
	internal class XDataTextBox : XDataControl
	{
		private TextBox _textBox = new TextBox() ;

		protected override void OnFieldIsSet()
		{
			base.OnFieldIsSet() ;

			_container.Children.Add( _textBox ) ;
			
			StringBuilder stringBuilder = new StringBuilder();

			foreach ( string text in Field.GetValues() )
			{
				stringBuilder.Append( text ) ;
			}
			
			_textBox.Text = stringBuilder.ToString() ;

			if ( Field.Type == FieldType.Text_Multi
				|| Field.Type == FieldType.Jid_Multi )
			{
				_textBox.MaxLines = 256 ;
			}
			else
			{
				_textBox.MaxLines = 1 ;
			}
		}
	}
}