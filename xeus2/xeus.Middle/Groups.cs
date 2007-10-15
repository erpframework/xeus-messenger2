using xeus2.xeus.Core;
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

        public void GroupsOpenUI(IContact contact)
        {
            GroupsWindow groupsWindow = new GroupsWindow(contact);

            groupsWindow.Activate();
            groupsWindow.ShowDialog();
        }

        public void GroupsOpen(IContact contact)
        {
            App.InvokeSafe(App._dispatcherPriority, new DisplayCallback(GroupsOpenUI), contact);
        }

        #region Nested type: DisplayCallback

        private delegate void DisplayCallback(IContact contact);

        #endregion
    }
}