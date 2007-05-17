using System.Windows.Controls ;
using agsXMPP.protocol.x.data ;

namespace xeus2.xeus.XData
{
	internal class XDataListSingle : XDataControl
	{
		private ComboBox _comboBox = new ComboBox() ;

		protected override void OnFieldIsSet()
		{
			base.OnFieldIsSet() ;

			_container.Children.Add( _comboBox ) ;

			foreach ( Option option in Field.GetOptions() )
			{
				ComboBoxItem comboBoxItem = new ComboBoxItem() ;

				comboBoxItem.Content = option.Label ;
				comboBoxItem.Tag = option.GetValue() ;

				foreach ( string text in Field.GetValues() )
				{
					if ( option.GetValue() == text )
					{
						comboBoxItem.IsSelected = true ;
						break ;
					}
				}

				_comboBox.Items.Add( comboBoxItem ) ;
			}
		}
	}
}