using System.Windows ;
using xeus2.xeus.Commands ;

namespace xeus2.xeus.UI
{
	/// <summary>
	/// Interaction logic for ServiceWindow.xaml
	/// </summary>
	public partial class ServiceWindow : Window
	{
		public ServiceWindow()
		{
			InitializeComponent() ;
		}

		public override void EndInit()
		{
			base.EndInit();

			ServiceCommands.RegisterCommands( this );
		}
	}
}