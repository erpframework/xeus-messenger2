using System.Windows.Controls ;
using agsXMPP.protocol.x.data ;
using xeus2.xeus.UI ;

namespace xeus2.xeus.XData
{
	internal class XDataListSingle : XDataControl
	{
		private ComboBox _comboBox = new ComboBox() ;

		protected override void OnInitialized( System.EventArgs e )
		{
			base.OnInitialized( e );

			_comboBox.Style = StyleManager.GetStyle( "XDataListSingle" ) ;
		}

		protected override void OnFieldIsSet()
		{
			base.OnFieldIsSet() ;

			_container.Children.Add( _comboBox ) ;

			foreach ( Option option in Field.GetOptions() )
			{
				ComboBoxItem comboBoxItem = new ComboBoxItem() ;

				comboBoxItem.Content = option.Label ;
				comboBoxItem.DataContext = option.GetValue() ;

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

		public override Field GetResult()
		{
			Field field = new Field( Field.Var, null, Field.Type );

			if ( _comboBox.SelectedItem != null )
			{
				field.SetValue( ( string )( ( ComboBoxItem ) _comboBox.SelectedItem ).DataContext ) ;
			}
			else
			{
				field.SetValue( null ) ;
			}

			return field ;
		}

		public override bool Validate()
		{
			return ( _comboBox.SelectedItem != null ) ;
		}
	}
}