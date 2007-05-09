using System;
using System.Collections.Generic;
using System.Text;

namespace xeus2.xeus.Middle
{
	internal class Notification
	{
		private static Notification _instance = new Notification() ;

		public static Notification Instance
		{
			get
			{
				return _instance ;
			}
		}
	}
}
