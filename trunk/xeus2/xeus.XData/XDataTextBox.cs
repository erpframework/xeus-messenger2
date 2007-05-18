using System.Text ;
using System.Windows.Controls ;
using agsXMPP.protocol.x.data ;
using xeus2.xeus.UI ;

namespace xeus2.xeus.XData
{
	internal class XDataTextBox : XDataControl
	{
		private TextBox _textBox = new TextBox() ;

		protected override void OnInitialized( System.EventArgs e )
		{
			base.OnInitialized( e );

			_textBox.Style = StyleManager.GetStyle( "XDataTextBox" ) ;
		}

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
			Field field = new Field( Field.Var, null, Field.Type );

			if ( Field.Type == FieldType.Text_Multi
				|| Field.Type == FieldType.Jid_Multi )
			{
				string [] values = new string[ _textBox.LineCount ];
 
				for ( int i = 0; i < _textBox.LineCount; i++ )
				{
					values[ i ] = _textBox.GetLineText( i ) ;
				}

				field.SetValues( values ) ;
			}
			else
			{
				field.SetValue( _textBox.Text ) ;
			}

			return field ;
		}

		public override bool Validate()
		{
			return ( !Field.IsRequired || _textBox.Text.Length > 0 ) ;
		}
	}
}