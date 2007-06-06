using System.Text.RegularExpressions ;

namespace xeus2.xeus.Utilities
{
	internal static class Validation
	{
		private const string _patternStrict = @"^(([^<>()[\]\\.,;:\s@\""]+"
		                                      + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
		                                      + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
		                                      + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
		                                      + @"[a-zA-Z]{2,}))$" ;

		private const string _patternStrict2 = @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
		                                      + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
		                                      + @"[a-zA-Z]{2,}))$" ;

		private static readonly Regex _reStrict = new Regex( _patternStrict ) ;
		private static readonly Regex _reStrict2 = new Regex( _patternStrict2 ) ;

		public static bool IsChatRoomValid( string jidBare )
		{
			return _reStrict.IsMatch( jidBare ) ;
		}

		public static bool IsCommandValid( string jidBare )
		{
			return _reStrict2.IsMatch( jidBare ) ;
		}
	}
}