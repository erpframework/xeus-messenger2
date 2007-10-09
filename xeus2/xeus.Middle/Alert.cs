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

        public bool? AlertOpen(string text, string resourceBrush)
        {
            UI.Alert wizard = new UI.Alert(text, resourceBrush ?? "error2_design");

            return wizard.ShowDialog();
        }
    }
}