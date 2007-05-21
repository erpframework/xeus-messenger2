using System.Data ;
using System.Windows.Controls ;
using System.Windows.Data ;
using agsXMPP.protocol.x.data ;
using xeus2.xeus.Core ;

namespace xeus2.xeus.XData
{
	/// <summary>
	/// Interaction logic for XDataResultView.xaml
	/// </summary>
	public partial class XDataResultView : UserControl
	{
		public XDataResultView()
		{
			InitializeComponent() ;
		}

		public void ReadFromXData( Data data )
		{
			DataTable table = new SearchResult( data ) ;
			_listView.DataContext = table ;

			Binding bind = new Binding() ;
			_listView.DataContext = table ;
			_listView.SetBinding( ListView.ItemsSourceProperty, bind ) ;
		}
	}
}