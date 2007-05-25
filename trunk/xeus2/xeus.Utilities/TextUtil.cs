using System;
using System.Collections.Generic;
using System.Globalization ;
using System.Text;
using System.Threading ;

namespace xeus2.xeus.Utilities
{
	internal static class TextUtil
	{
		static CultureInfo _cultureInfo = Thread.CurrentThread.CurrentCulture ;

		public static string ToTitleCase( string text )
		{
			return _cultureInfo.TextInfo.ToTitleCase( text ) ;
		}
	}
}
