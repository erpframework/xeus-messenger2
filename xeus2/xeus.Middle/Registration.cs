using agsXMPP.protocol.iq.register;
using xeus2.xeus.Core;
using xeus2.xeus.UI;

namespace xeus2.xeus.Middle
{
    internal class Registration
    {
        private static readonly Registration _instance = new Registration();

        public static Registration Instance
        {
            get
            {
                return _instance;
            }
        }

        protected void InBandRegistration(Register register, Service service)
        {
            try
            {
                UI.Registration registration = new UI.Registration(register, service);
                registration.Show();
            }

            catch (WindowExistsException e)
            {
                e.ActivateControl();
            }
        }

        public void DisplayInBandRegistration(Register register, Service service)
        {
            App.InvokeSafe(App._dispatcherPriority,
                           new DisplayCallback(InBandRegistration), register, service);
        }

        #region Nested type: DisplayCallback

        private delegate void DisplayCallback(Register register, Service service);

        #endregion
    }
}