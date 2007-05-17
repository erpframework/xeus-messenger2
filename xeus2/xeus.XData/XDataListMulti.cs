using System ;
using System.Collections.Generic ;
using System.Windows ;
using System.Windows.Controls ;
using agsXMPP.protocol.x.data ;
using xeus2.xeus.UI ;

namespace xeus2.xeus.XData
{
	internal class XDataListMulti : XDataControl
	{
		private StackPanel _panel = new StackPanel() ;

		protected override void OnInitialized( EventArgs e )
		{
			base.OnInitialized( e ) ;

			_panel.Style = StyleManager.GetStyle( "XDataListMulti" ) ;
		}

		protected override void OnFieldIsSet()
		{
			base.OnFieldIsSet() ;

			_container.Children.Add( _panel ) ;

			foreach ( Option option in Field.GetOptions() )
			{
				CheckBox checkBox = new CheckBox() ;

				checkBox.Content = option.Label ;
				checkBox.Tag = option.GetValue() ;
				checkBox.Style = StyleManager.GetStyle( "XDataCheckBox" ) ;

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

		public override Field GetResult()
		{
			List< string > values = new List< string >() ;

			foreach ( UIElement element in _panel.Children )
			{
				CheckBox checkBox = element as CheckBox ;

				if ( checkBox != null && ( bool ) checkBox.IsChecked )
				{
					values.Add( ( string ) checkBox.Tag ) ;
				}
			}

			Field.SetValues( values.ToArray() ) ;

			return Field ;
		}
	}
}