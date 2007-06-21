using System;
using System.Collections.Generic;
using System.Text;
using System.Windows ;
using System.Windows.Media;

namespace xeus2.xeus.UI
{
	internal static class StyleManager
	{
		public static Style GetStyle( string style )
		{
			return ( Style )App.Current.FindResource( style ) ;
		}
    
        public static DrawingBrush GetBrush(string name)
        {
            return ( DrawingBrush )App.Current.FindResource( name ) ;
        }
    }
}
