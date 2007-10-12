using xeus2.xeus.UI;

namespace xeus2.xeus.Middle
{
    internal class Groups
    {
        private static readonly Groups _instance = new Groups();

        public static Groups Instance
        {
            get
            {
                return _instance;
            }
        }

        public void GroupsOpenUI()
        {
            GroupsWindow groupsWindow = new GroupsWindow();

            groupsWindow.Activate();
            groupsWindow.ShowDialog();
        }

        public void GroupsOpen()
        {
            App.InvokeSafe(App._dispatcherPriority, new DisplayCallback(GroupsOpenUI));
        }

        #region Nested type: DisplayCallback

        private delegate void DisplayCallback();

        #endregion
    }
}