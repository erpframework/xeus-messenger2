using System.Windows.Controls ;
using agsXMPP.protocol.x.data ;

namespace xeus2.xeus.XData
{
	internal class XDataListMulti : XDataControl
	{
		private StackPanel _panel = new StackPanel() ;

		protected override void OnFieldIsSet()
		{
			base.OnFieldIsSet() ;

			_container.Children.Add( _panel ) ;

			foreach ( Option option in Field.GetOptions() )
			{
				CheckBox checkBox = new CheckBox() ;

				checkBox.Content = option.Label ;
				checkBox.DataContext = option.GetValue() ;

				foreach ( string text in Field.GetValues() )
				{
					if ( option.GetValue() == text )
					{
						checkBox.IsChecked = true ;
						break ;
					}
				}

				_panel.Children.Add( checkBox ) ;
			}
		}
	}
}