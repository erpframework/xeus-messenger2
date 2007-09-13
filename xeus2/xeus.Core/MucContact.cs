using System;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.protocol.x.muc;
using xeus2.Properties;

namespace xeus2.xeus.Core
{
    internal class MucContact : NotifyInfoDispatcher, IJid
    {
        private readonly MucRoom _mucRoom;
        private Presence _presence;

        public MucContact(Presence presence, MucRoom mucRoom)
        {
            _mucRoom = mucRoom;

            Presence = presence;
        }

        public string UserJidText
        {
            get
            {
                if (Presence != null
                    && Presence.MucUser != null
                    && Presence.MucUser.Item != null
                    && Presence.MucUser.Item.Jid != null)
                {
                    return Presence.MucUser.Item.Jid.ToString();
                }
                else
                {
                    return "Undisclosed";
                }
            }
        }

        public Jid Jid
        {
            get
            {
                if (Presence != null
                    && Presence.MucUser != null
                    && Presence.MucUser.Item != null)
                {
                    return Presence.MucUser.Item.Jid;
                }
                else
                {
                    return null;
                }
            }
        }

        public Jid MucJid
        {
            get
            {
                return Presence.From;
            }
        }

        public Role Role
        {
            get
            {
                return Presence.MucUser.Item.Role;
            }
        }

        public string MucStatusCode
        {
            get
            {
                if (_presence.MucUser.Status != null)
                {
                    return _presence.MucUser.Status.Code.ToString();
                }
                else
                {
                    return null;
                }
            }
        }

        public string MucStatusCodeText
        {
            get
            {
                if (_presence.MucUser.Status != null)
                {
                    return MucStatusCodeTexts.GetCodeText(_presence.MucUser.Status);
                }
                else
                {
                    return null;
                }
            }
        }

        public string StatusText
        {
            get
            {
                if (string.IsNullOrEmpty(Presence.Status))
                {
                    return Resources.Constant_NoMessage;
                }
                else
                {
                    return Presence.Status;
                }
            }
        }

        public ShowType Show
        {
            get
            {
                return Presence.Show;
            }
        }

        public Affiliation Affiliation
        {
            get
            {
                return Presence.MucUser.Item.Affiliation;
            }
        }

        public string Group
        {
            get
            {
                switch (Role)
                {
                    case Role.visitor:
                        {
                            return Resources.Role_Visitor;
                        }
                    case Role.participant:
                        {
                            return Resources.Role_Participant;
                        }
                    case Role.moderator:
                        {
                            return Resources.Role_Moderator;
                        }
                    default:
                        {
                            return Resources.Role_None;
                        }
                }
            }
        }

        public string Nick
        {
            get
            {
                if (Jid == null)
                {
                    return MucJid.Resource;
                }
                else
                {
                    return Presence.MucUser.Item.Nickname;
                }
            }
        }

        public Presence Presence
        {
            get
            {
                return _presence;
            }
            set
            {
                _presence = value;

                NotifyPropertyChanged("Nick");
                NotifyPropertyChanged("Group");
                NotifyPropertyChanged("Role");
                NotifyPropertyChanged("Affiliation");
                NotifyPropertyChanged("StatusText");
                NotifyPropertyChanged("Show");
            }
        }

        public MucRoom MucRoom
        {
            get
            {
                return _mucRoom;
            }
        }

        public override string ToString()
        {
            return Nick;
        }
    }
}