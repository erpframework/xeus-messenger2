using System;
using System.Collections.Generic;
using System.Text;

namespace xeus2.xeus.Core
{
	internal class XeusException : Exception
	{
		public XeusException( string reason ) : base( reason )
		{
		}
	}
}
