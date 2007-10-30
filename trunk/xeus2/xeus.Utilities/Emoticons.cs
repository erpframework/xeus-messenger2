using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using xeus2.xeus.UI;

namespace xeus2.xeus.Utilities
{
    internal class Emoticons
    {
        readonly Dictionary<string, string> _emos = new Dictionary<string, string>(50);
        private readonly string[] _emosplits;

        public Emoticons()
        {
            _emos.Add("0=)", "face-angel");
            _emos.Add("^j^", "face-angel");
            _emos.Add("O:-)", "face-angel");
            _emos.Add("O:)", "face-angel");
            _emos.Add("O-)", "face-angel");
            _emos.Add("(a)", "face-angel");

            _emos.Add(":_ (", "face-crying");
            _emos.Add(":'(", "face-crying");
            _emos.Add("T_T", "face-crying");
            _emos.Add(":*(", "face-crying");
            _emos.Add(":((", "face-crying");
            _emos.Add(";'-(", "face-crying");
            _emos.Add(";'(", "face-crying");

            _emos.Add(":-D", "face-grin");
            _emos.Add(":D", "face-grin");
            _emos.Add(":->", "face-grin");

            _emos.Add(":evil:", "face-devil-grin");

            _emos.Add("-@--@-", "face-glasses");
            _emos.Add("8-|", "face-glasses");

            _emos.Add(":-*", "face-kiss");
            _emos.Add(":-)*", "face-kiss");

            _emos.Add(":|", "face-plain");
            _emos.Add(":-|", "face-plain");

            _emos.Add(":(", "face-sad");
            _emos.Add(":-(", "face-sad");
            _emos.Add("=(", "face-sad");

            _emos.Add(":))", "face-smile-big");
            _emos.Add(":-))", "face-smile-big");

            _emos.Add(":-)", "face-smile");
            _emos.Add(":)", "face-smile");

            _emos.Add(":-O", "face-surprise");
            _emos.Add(":O", "face-surprise");
            _emos.Add("*SURPRISED*", "face-surprise");

            _emos.Add(";-)", "face-wink");
            _emos.Add(";)", "face-wink");

            _emosplits = new string[_emos.Count];
            _emos.Keys.CopyTo(_emosplits, 0);
        }

        public string[] SplitText(string text)
        {
            return text.Split(_emosplits, StringSplitOptions.RemoveEmptyEntries);
        }

        Brush GetEmoBrush(string emo)
        {
            Brush brush = StyleManager.GetBrush("time_now_design");

            return brush;
        }
    }
}
