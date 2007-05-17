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
				_textBox.MinLines = 3 ;
				_textBox.AcceptsReturn = true ;
			}
			else
			{
				_textBox.MinLines = 1 ;
				_textBox.MaxLines = 1 ;
				_textBox.AcceptsReturn = false ;
			}
		}

		public override Field GetResult()
		{
			if ( Field.Type == FieldType.Text_Multi
				|| Field.Type == FieldType.Jid_Multi )
			{
				string [] values = new string[ _textBox.LineCount ];
 
				for ( int i = 0; i < _textBox.LineCount; i++ )
				{
					values[ i ] = _textBox.GetLineText( i ) ;
				}

				Field.SetValues( values ) ;
			}
			else
			{
				Field.SetValue( _textBox.Text ) ;
			}

			return Field ;
		}
	}
}