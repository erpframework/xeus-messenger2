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

        public bool? AlertOpenUI(string text, string resourceBrush)
        {
            UI.Alert wizard = new UI.Alert(text, resourceBrush ?? "error2_design");

            wizard.Activate();
            return wizard.ShowDialog();
        }

        public void AlertOpen(string text, string resourceBrush)
        {
            App.InvokeSafe(App._dispatcherPriority,
                           new DisplayCallback(AlertOpenUI), text, resourceBrush);
        }

        #region Nested type: DisplayCallback

        private delegate bool? DisplayCallback(string text, string resourceBrush);

        #endregion
    }
}