using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using xeus2.xeus.Core ;

namespace xeus2.xeus.UI
{
	/// <summary>
	/// Interaction logic for Search.xaml
	/// </summary>

	public partial class Search : System.Windows.Window
	{
		internal Search( agsXMPP.protocol.iq.search.Search search, Service service )
		{
			InitializeComponent() ;

			_search.Setup( search, service ) ;
		}

		protected void OnSearch( object sender, RoutedEventArgs eventArgs )
		{
			if ( !_search.IsValid )
			{
				return ;
			}

			if ( _search.XData != null )
			{
				Account.Instance.DoSearchService( _search.Service, _search.GetResult() ) ;
			}
			else
			{
				_search.UpdateData();
	
				Account.Instance.DoSearchService( _search.Service, _search.FirstName,
				                                    _search.LastName, _search.Nickname,
													_search.Email ) ;
			}
		}
	}
}