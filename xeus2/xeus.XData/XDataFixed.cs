using System.Text ;
using System.Windows.Controls ;
using agsXMPP.protocol.x.data ;

namespace xeus2.xeus.XData
{
	internal class XDataFixed : XDataControl
	{
		private TextBlock _textBlock = new TextBlock() ;

		protected override void OnFieldIsSet()
		{
			base.OnFieldIsSet() ;

			_container.Children.Add( _textBlock ) ;

			StringBuilder stringBuilder = new StringBuilder() ;

			foreach ( string text in Field.GetValues() )
			{
				stringBuilder.Append( text ) ;
			}

			_textBlock.Text = stringBuilder.ToString() ;
		}

		public override Field GetResult()
		{
			return Field ;
		}
	}
}