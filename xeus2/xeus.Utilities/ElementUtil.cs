using System;
using System.Collections.Generic;
using System.Text;
using agsXMPP.protocol.x.data ;
using agsXMPP.Xml.Dom ;

namespace xeus2.xeus.Utilities
{
	internal static class ElementUtil
	{
        public static agsXMPP.protocol.x.data.Data GetData(Element element)
		{
            agsXMPP.protocol.x.data.Data data = null;

			if ( element.HasChildElements )
			{
				foreach ( Node node in element.ChildNodes )
				{
                    if (node is agsXMPP.protocol.x.data.Data)
					{
                        data = (agsXMPP.protocol.x.data.Data)node;
						break ;
					}
				}
			}

			return data ;
		}
	}
}
