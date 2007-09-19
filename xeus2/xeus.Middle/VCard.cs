using System;
using System.Collections.Generic;
using System.Text;
using xeus2.xeus.Core;
using xeus2.xeus.UI;

namespace xeus2.xeus.Middle
{
    internal class VCard
    {
        private static readonly VCard _instance = new VCard();

        private delegate void DisplayVCardCallback(IContact contact);

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
            }

            catch (WindowExistsException e)
            {
                e.ActivateControl();
            }
        }
    }
}
