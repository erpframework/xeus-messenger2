using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading ;
using xeus2.xeus.Core ;

namespace xeus2.xeus.Middle
{
	internal class Search
	{
		private delegate void DisplayCallback( agsXMPP.protocol.iq.search.Search search, Service service ) ;

		private static Search _instance = new Search() ;

		public static Search Instance
		{
			get
			{
				return _instance ;
			}
		}

		protected void SearchService( agsXMPP.protocol.iq.search.Search search, Service service )
		{
			UI.Search searchControl = new UI.Search( search, service ) ;

			searchControl.Show() ;
		}

		public void DisplaySearch( agsXMPP.protocol.iq.search.Search Search, Service service )
		{
			App.InvokeSafe( DispatcherPriority.Normal,
			                new DisplayCallback( SearchService ), Search, service ) ;
		}
	}
}
