using xeus2.xeus.Core;
using xeus2.xeus.UI;

namespace xeus2.xeus.Middle
{
    internal class Wizard
    {
        private static readonly Wizard _instance = new Wizard();

        public static Wizard Instance
        {
            get
            {
                return _instance;
            }
        }

        protected void WizardOpen()
        {
            try
            {
                UI.Wizard wizard = new UI.Wizard();
                wizard.Show();
            }

            catch (WindowExistsException e)
            {
                e.ActivateControl();
            }
        }

        protected void AddContactOpen()
        {
            try
            {
                AddContact wizard = new AddContact();
                wizard.Show();

                Account.Instance.DiscoverRegistereServices();
            }

            catch (WindowExistsException e)
            {
                e.ActivateControl();
            }
        }

        public void DisplayWizard()
        {
            App.InvokeSafe(App._dispatcherPriority,
                           new DisplayCallback(WizardOpen));
        }

        public void DisplayAddContact()
        {
            App.InvokeSafe(App._dispatcherPriority,
                           new DisplayCallback(AddContactOpen));
        }

        #region Nested type: DisplayCallback

        private delegate void DisplayCallback();

        #endregion
    }
}