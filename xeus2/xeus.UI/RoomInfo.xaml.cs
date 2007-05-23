using System.Windows ;
using xeus2.xeus.Core ;

namespace xeus2.xeus.UI
{
	/// <summary>
	/// Interaction logic for RoomInfo.xaml
	/// </summary>
	public partial class RoomInfo : Window
	{
		internal RoomInfo( Service service )
		{
			InitializeComponent() ;

			_detail.Setup( service ) ;
		}
	}
}