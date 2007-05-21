using System.Data ;
using System.Windows.Controls ;
using System.Windows.Data ;
using agsXMPP.protocol.iq.search ;
using agsXMPP.protocol.x.data ;
using xeus2.xeus.Core ;
using xeus2.xeus.Utilities ;

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

		public void ReadSearchResult( Search search )
		{
			if ( search.Data != null )
			{
				ReadFromXData( search.Data ) ;
			}
			else
			{
				ReadSimple( search.GetItems() ) ;
			}
		}

		protected void ReadSimple( SearchItem [] items )
		{
			DataTable table = new SearchResult( items ) ;
			_listView.DataContext = table ;

			XDataSearchResultHeader gridView = new XDataSearchResultHeader() ;
			_listView.View = gridView ;

			Binding bind = new Binding() ;
			_listView.DataContext = table ;
			_listView.SetBinding( ListView.ItemsSourceProperty, bind ) ;
		}

		protected void ReadFromXData( Data data )
		{
			DataTable table = new SearchResult( data.Reported, data.GetItems() ) ;
			_listView.DataContext = table ;

			XDataSearchResultHeader gridView = new XDataSearchResultHeader( data.Reported ) ;
			_listView.View = gridView ;

			Binding bind = new Binding() ;
			_listView.DataContext = table ;
			_listView.SetBinding( ListView.ItemsSourceProperty, bind ) ;
		}
	}
}