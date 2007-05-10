using System;
using System.Collections.Generic;
using System.Text;
using FastDynamicPropertyAccessor ;
using xeus2.xeus.Core ;

namespace xeus2.xeus.Utilities
{
	static internal class DisplayNameBuilder
	{
		public static string GetDisplayName( IContact contact )
		{
			PropertyAccessor propertyAccessor = 
				new PropertyAccessor( typeof( IContact ), "LastName" );

			object [] properties = new object[ 1 ];

			properties[ 0 ] = propertyAccessor.Get( contact );

			return string.Format( "{0}", properties );
		}
	}
}
