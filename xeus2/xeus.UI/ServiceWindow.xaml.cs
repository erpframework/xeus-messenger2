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

            Loaded += new RoutedEventHandler(ServiceWindow_Loaded);
		}

        void ServiceWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Vista.MakeVistaFrame(this, (int)topBar.ActualHeight);
        }

		public override void EndInit()
		{
			base.EndInit();

			ServiceCommands.RegisterCommands( this );
		}
	}
}