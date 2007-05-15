using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls ;

namespace xeus2.xeus.XData
{
	internal class XDataTextBox : XDataControl
	{
		TextBox _textBox = new TextBox();

		protected override void OnFieldIsSet()
		{
			base.OnFieldIsSet();

			_container.Children.Add( _textBox ) ;
			_textBox.Text = Field.Value ;
		}
	}
}
