using System;
using System.Collections.Generic;
using System.Text;
using System.Windows ;
using System.Windows.Threading ;
using xeus2.xeus.Core ;
using xeus2.xeus.UI;

namespace xeus2.xeus.Middle
{
	internal class Search
	{
		private delegate void DisplayCallback( agsXMPP.protocol.iq.search.Search search, Service service ) ;

		private static readonly Search _instance = new Search() ;

		public static Search Instance
		{
			get
			{
				return _instance ;
			}
		}

		protected void SearchService( agsXMPP.protocol.iq.search.Search search, Service service )
		{
            try
            {
                UI.Search searchWindow = new UI.Search(search, service);

                searchWindow.Show();
                searchWindow.Activate();
            }

		    catch ( WindowExistsException e )
			{
			    e.ActivateControl();
			}
		}

		public void DisplaySearch( agsXMPP.protocol.iq.search.Search Search, Service service )
		{
            App.InvokeSafe(App._dispatcherPriority,
			                new DisplayCallback( SearchService ), Search, service ) ;
		}

		protected void SearchServiceResult( agsXMPP.protocol.iq.search.Search search, Service service )
		{
            BaseWindow window = WindowManager.Find(WindowManager.MakeKey(UI.Search._keyBase, service.Jid.Bare)) as BaseWindow;

            if (window != null)
			{
                ((UI.Search)window).DisplaySearchResult(search, service);
                window.Show();
                window.Activate();
			}
		}

		public void DisplaySearchResult( agsXMPP.protocol.iq.search.Search Search, Service service )
		{
            App.InvokeSafe(App._dispatcherPriority,
			                new DisplayCallback( SearchServiceResult ), Search, service ) ;
		}
	}
}
