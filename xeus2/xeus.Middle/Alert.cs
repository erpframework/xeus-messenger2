namespace xeus2.xeus.Middle
{
    internal class Alert
    {
        private static readonly Alert _instance = new Alert();

        public static Alert Instance
        {
            get
            {
                return _instance;
            }
        }

        public UI.Alert.Buttons AlertOpenUI(string text, string resourceBrush, UI.Alert.Buttons buttons)
        {
            UI.Alert wizard = new UI.Alert(text, resourceBrush ?? "error2_design", buttons);

            wizard.Activate();
            wizard.ShowDialog();

            return wizard.Return;
        }

        public void AlertOpen(string text, string resourceBrush, UI.Alert.Buttons buttons)
        {
            App.InvokeSafe(App._dispatcherPriority,
                           new DisplayCallback(AlertOpenUI), text, resourceBrush, buttons);
        }

        #region Nested type: DisplayCallback

        private delegate UI.Alert.Buttons DisplayCallback(string text, string resourceBrush, UI.Alert.Buttons buttons);

        #endregion
    }
}