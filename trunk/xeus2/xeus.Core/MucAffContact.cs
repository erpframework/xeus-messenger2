using agsXMPP;
using agsXMPP.protocol.x.muc;
using Item = agsXMPP.protocol.x.muc.iq.admin.Item;

namespace xeus2.xeus.Core
{
    internal class MucAffContact
    {
        public string Jid
        {
            get
            {
                return _item.Jid.ToString();
            }
        }

        public Affiliation Affiliation
        {
            get
            {
                return _item.Affiliation;
            }
        }

        private readonly Item _item;

        public MucAffContact(Item item)
        {
            _item = item;
        }

        public MucAffContact(Jid jid, Affiliation affiliation)
        {
            _item = new Item(affiliation);
            _item.Jid = jid;
        }

        public override string ToString()
        {
            return Jid;
        }
    }
}