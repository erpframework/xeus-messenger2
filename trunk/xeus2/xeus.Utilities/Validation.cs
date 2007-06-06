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

		private static readonly Regex _reStrict = new Regex( _patternStrict ) ;

		public static bool IsChatRoomValid( string jidBare )
		{
			return _reStrict.IsMatch( jidBare ) ;
		}
	}
}