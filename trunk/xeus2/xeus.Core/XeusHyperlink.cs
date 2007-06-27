using System.Diagnostics;
using System.Windows.Documents;

namespace xeus2.xeus.Core
{
    internal class XeusHyperlink : Hyperlink
    {
        public XeusHyperlink(Inline inline)
            : base(inline)
        {
        }

        protected override void OnClick()
        {
            string target = NavigateUri.AbsoluteUri;

            try
            {
                Process.Start(target);
            }

            catch
            {
            }
        }
    }
}