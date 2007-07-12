using System.Globalization ;
using System.Threading ;

namespace xeus2.xeus.Utilities
{
	internal static class TextUtil
	{
		private static CultureInfo _cultureInfo = Thread.CurrentThread.CurrentCulture ;

		public static string ToTitleCase( string text )
		{
			return _cultureInfo.TextInfo.ToTitleCase( text ) ;
		}

	    private const string _wordBoundaries = @" ;:\r\n\t<>.?/\+-*()[]{},";

        public static bool ContainsNick(string nick, string text)
        {
            for (int index = -1; ; )
            {
                index = text.IndexOf(nick, index + 1);

                char before = '\0';
                char after = '\0';

                if (index >= 0)
                {
                    if (index > 0)
                    {
                        before = text[index - 1];
                    }

                    if (index < text.Length - nick.Length)
                    {
                        after = text[index + nick.Length];
                    }

                    if ((before == '\0'
                            || _wordBoundaries.IndexOf(before) >= 0)
                        &&
                            (after == '\0'
                            || _wordBoundaries.IndexOf(after) >= 0)
                        )
                    {
                        return true;
                    }
                }
                else
                {
                    break;
                }
            }

            return false;
        }
	}
}