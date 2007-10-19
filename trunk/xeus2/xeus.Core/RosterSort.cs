using System.Collections;

namespace xeus2.xeus.Core
{
    internal class RosterSort : IComparer
    {
        #region IComparer Members

        public int Compare(object x, object y)
        {
            IContact itemX = (IContact) x;
            IContact itemY = (IContact) y;

            if (itemX.Group == itemY.Group)
            {
                return string.Compare(itemX.DisplayName, itemY.DisplayName, true);
            }
            else
            {
                bool isSysGroupX = Roster.Instance.IsGroupExpanded(itemX.Group);
                bool isSysGroupY = Roster.Instance.IsGroupExpanded(itemY.Group);

                if (isSysGroupX == isSysGroupY)
                {
                    return string.Compare(itemX.Group, itemY.Group);
                }
                else
                {
                    return (isSysGroupX) ? 1 : -1;
                }
            }
        }

        #endregion
    }
}