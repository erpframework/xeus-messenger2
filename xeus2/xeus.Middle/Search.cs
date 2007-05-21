using System;
using System.Collections.Generic;
using System.Text;
using System.Windows ;
using System.Windows.Threading ;
using xeus2.xeus.Core ;

namespace xeus2.xeus.Middle
{
	internal class Search : WindowManager< Service, UI.Search >
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
			UI.Search searchWindow = GetWindow( service ) ;

			if ( searchWindow == null )
			{
				searchWindow = new UI.Search( search, service ) ;
				searchWindow.DataContext = service ;
				AddWindow( service, searchWindow );
			}

			searchWindow.Closing += new System.ComponentModel.CancelEventHandler( searchWindow_Closing );
			searchWindow.Show() ;
		}

		void searchWindow_Closing( object sender, System.ComponentModel.CancelEventArgs e )
		{
			RemoveWindow( ( ( Window )sender ).DataContext as Service );
			( ( Window ) sender ).Closing -= searchWindow_Closing ;
		}

		public void DisplaySearch( agsXMPP.protocol.iq.search.Search Search, Service service )
		{
			App.InvokeSafe( DispatcherPriority.Normal,
			                new DisplayCallback( SearchService ), Search, service ) ;
		}

		protected void SearchServiceResult( agsXMPP.protocol.iq.search.Search search, Service service )
		{
			UI.Search searchWindow = GetWindow( service ) ;

			if ( searchWindow != null )
			{
				searchWindow.DisplaySearchResult( search, service );
				searchWindow.Show() ;
			}
		}

		public void DisplaySearchResult( agsXMPP.protocol.iq.search.Search Search, Service service )
		{
			App.InvokeSafe( DispatcherPriority.Normal,
			                new DisplayCallback( SearchServiceResult ), Search, service ) ;
		}
	}
}
