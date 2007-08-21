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

        public MucAffContacts MucAffContacts
        {
            get
            {
                return _mucAffContacts;
            }
        }

        readonly MucAffContacts _mucAffContacts;

        public MucAffContact(Item item, MucAffContacts mucAffContacts)
        {
            _item = item;
            _mucAffContacts = mucAffContacts;
        }

        public MucAffContact(Jid jid, Affiliation affiliation, MucAffContacts mucAffContacts)
        {
            _mucAffContacts = mucAffContacts;
            _item = new Item(affiliation);
            _item.Jid = jid;
        }

        public override string ToString()
        {
            return Jid;
        }
    }
}