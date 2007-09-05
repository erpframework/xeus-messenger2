using System.Collections;
using agsXMPP.protocol.client;

namespace xeus2.xeus.Utilities
{
    internal class PresenceCompare : IComparer
    {
        #region IComparer Members

        public int Compare(object x, object y)
        {
            if (x == y)
            {
                return 0;
            }

            if (x == null)
            {
                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            Presence presence = (Presence) x;
            Presence presenceY = (Presence) y;

            int ptX = GetPointsOfPresence(presence);
            int ptY = GetPointsOfPresence(presenceY);

            if (ptX != ptY)
            {
                return (ptX > ptY) ? 1 : -1;
            }
            else
            {
                ptX += GetPointsOfShowType(presence);
                ptY += GetPointsOfShowType(presenceY);

                if (ptX != ptY)
                {
                    return (ptX > ptY) ? 1 : -1;
                }

                if (presence.Priority == presenceY.Priority)
                {
                    return 0;
                }
                else
                {
                    return (presence.Priority > presenceY.Priority) ? 1 : -1;
                }
            }
        }

        #endregion

        private static int GetPointsOfShowType(Presence presence)
        {
            if (presence == null)
            {
                return 0;
            }

            if (presence.Type == PresenceType.available)
            {
                switch (presence.Show)
                {
                    case ShowType.dnd:
                        {
                            return 3;
                        }
                    case ShowType.xa:
                        {
                            return 4;
                        }
                    case ShowType.away:
                        {
                            return 5;
                        }
                    default:
                        {
                            return 6;
                        }
                }
            }
            else
            {
                return 0;
            }
        }

        private static int GetPointsOfPresence(Presence presence)
        {
            if (presence == null || presence.Type == PresenceType.unavailable)
            {
                return 0;
            }

            if (presence.Type == PresenceType.invisible)
            {
                return 1;
            }

            if (presence.Type == PresenceType.available)
            {
                return 2;
            }

            return 0;
        }
    }
}