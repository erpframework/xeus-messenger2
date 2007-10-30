using xeus2.xeus.Core;
using xeus2.xeus.UI;

namespace xeus2.xeus.Middle
{
    internal class VCard
    {
        private static readonly VCard _instance = new VCard();

        public static VCard Instance
        {
            get
            {
                return _instance;
            }
        }

        public void DisplayVCard(IContact contact)
        {
            App.InvokeSafe(App._dispatcherPriority,
                           new DisplayVCardCallback(DisplayVCardInternal), contact);
        }

        private void DisplayVCardInternal(IContact contact)
        {
            try
            {
                VCardDisplay vCardDisplay = new VCardDisplay(contact);
                vCardDisplay.Show();
                vCardDisplay.Activate();
            }

            catch (WindowExistsException e)
            {
                e.ActivateControl();
            }
        }

        #region Nested type: DisplayVCardCallback

        private delegate void DisplayVCardCallback(IContact contact);

        #endregion
    }
}