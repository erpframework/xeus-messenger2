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
				comboBoxItem.DataContext = option.GetValue() ;

				if ( option.GetValue() == Field.Value )
				{
					comboBoxItem.IsSelected = true ;
				}

				_comboBox.Items.Add( comboBoxItem ) ;
			}
		}
	}
}