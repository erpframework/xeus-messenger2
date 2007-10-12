using xeus2.xeus.Data;

namespace xeus2.xeus.Core
{
    internal class Groups : ObservableCollectionDisp<Group>
    {
        public void LoadState()
        {
            Database.ReadGroups(this);
        }

        public void SaveState()
        {
            Database.StoreGroups(this);
        }

        public Group FindGroup(string name)
        {
            foreach (Group group in this)
            {
                if (group.Name == name)
                {
                    return group;
                }
            }

            return null;
        }
    }
}