using System;
using System.Collections.Generic;
using System.Text;
using agsXMPP.protocol.x.data ;
using agsXMPP.Xml.Dom ;

namespace xeus2.xeus.Utilities
{
	internal static class ElementUtil
	{
		public static Data GetData( Element element )
		{
			Data data = null ;

			if ( element.HasChildElements )
			{
				foreach ( Node node in element.ChildNodes )
				{
					if ( node is Data )
					{
						data = node as Data ;
						break ;
					}
				}
			}

			return data ;
		}
	}
}
