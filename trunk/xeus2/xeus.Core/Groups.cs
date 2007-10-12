using xeus2.xeus.Data;

namespace xeus2.xeus.Core
{
    internal class Groups
    {
        private readonly ObservableCollectionDisp<Group> _items = new ObservableCollectionDisp<Group>();

        public ObservableCollectionDisp<Group> Items
        {
            get
            {
                return _items;
            }
        }

        public void LoadState()
        {
            Database.ReadGroups(_items);
        }

        public void SaveState()
        {
            Database.StoreGroups(_items);
        }

        public Group FindGroup(string name)
        {
            foreach (Group group in _items)
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