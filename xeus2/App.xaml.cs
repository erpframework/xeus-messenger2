using System.Globalization ;
using System.Resources ;
using System.Windows ;
using xeus2.Properties ;

namespace xeus2
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			//xeus2.Properties.Resources.Culture = new CultureInfo( "en-US" ) ;

			string h = xeus2.Properties.Resources.Event_PresenceChange ;
		}
	}
}