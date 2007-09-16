using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using agsXMPP.protocol.iq.disco;

namespace xeus2.xeus.Utilities
{
    internal static class TextUtil
    {
        private const string _wordBoundaries = " ;:\r\n\t<>.?/\\+-*()[]{},";
        private static readonly CultureInfo _cultureInfo = Thread.CurrentThread.CurrentCulture;

        public static string ToTitleCase(string text)
        {
            return _cultureInfo.TextInfo.ToTitleCase(text);
        }

        public static bool ContainsNick(string nick, string text)
        {
            for (int index = -1;;)
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

        public static string GetWordFromBack(string text, int position)
        {
            for (int i = position - 1; i >= 0; i--)
            {
                if (_wordBoundaries.IndexOf(text[i]) >= 0)
                {
                    return text.Substring(i + 1, position - (i + 1));
                }
            }

            return text.Substring(0, position);
        }

        public static string HexEncode(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder(bytes.Length);

            for (int i = 0; i < bytes.Length; i++)
            {
                builder.AppendFormat("{0:x2}", bytes[i]);
            }

            return builder.ToString();
        }

        public static string GenerateVerAttribute(DiscoInfo info)
        {
            StringBuilder builder = new StringBuilder(256);

            DiscoIdentity[] identities = info.GetIdentities();

            string[] ids = new string[identities.Length];

            for (int i = 0; i < identities.Length; i++)
            {
                ids[i] = string.Format("{0}/{1}", identities[i].Category, identities[i].Type);
            }

            Array.Sort(ids);

            foreach (string id in ids)
            {
                builder.AppendFormat("{0}<", id);
            }

            DiscoFeature[] features = info.GetFeatures();

            string[] feas = new string[features.Length];

            for (int i = 0; i < features.Length; i++)
            {
                feas[i] = features[i].Var;
            }

            Array.Sort(feas);

            foreach (string fea in feas)
            {
                builder.AppendFormat("{0}<", fea);
            }

            SHA1Managed sha1 = new SHA1Managed();

            byte[] hash = sha1.ComputeHash(Encoding.Unicode.GetBytes(builder.ToString()));

            return Convert.ToBase64String(hash);
        }
    }
}