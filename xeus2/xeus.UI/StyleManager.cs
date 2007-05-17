using System;
using System.Collections.Generic;
using System.Text;
using System.Windows ;

namespace xeus2.xeus.UI
{
	internal static class StyleManager
	{
		public static Style GetStyle( string style )
		{
			return ( Style )App.Current.FindResource( style ) ;
		}
	}
}
