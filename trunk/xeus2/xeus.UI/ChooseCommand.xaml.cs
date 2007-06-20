using System.Windows ;
using xeus2.xeus.Core ;

namespace xeus2.xeus.UI
{
	/// <summary>
	/// Interaction logic for ChooseCommand.xaml
	/// </summary>
	public partial class ChooseCommand : Window
	{
		public ChooseCommand()
		{
			InitializeComponent() ;
		}

		internal Service Service
		{
			get
			{
				return _listCommands.SelectedItem as Service ;
			}
		}

		private void OnExecute(object sender, RoutedEventArgs args)
		{
			DialogResult = true;
		}

        private void OnResultSelectionDoubleClicked( object sender, RoutedEventArgs args )
        {
            if ( _listCommands.SelectedItems.Count > 0 )
            {
                DialogResult = true;
                Close();
            }
        }
	}
}