using System.Data ;
using agsXMPP.protocol.x.data ;
using agsXMPP.Xml.Dom ;

namespace xeus2.xeus.Core
{
	internal class SearchResult : DataTable
	{
		public SearchResult( Data data )
		{
			foreach ( Node node in data.ChildNodes )
			{
				Field field = node as Field ;

				if ( field != null )
				{
					Columns.Add( "name", typeof ( string ) ) ;
				}
			}
		}
	}
}