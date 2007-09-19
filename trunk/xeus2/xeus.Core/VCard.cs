using System;
using agsXMPP;
using agsXMPP.protocol.iq.vcard;

namespace xeus2.xeus.Core
{
    public class VCard
    {
        private readonly Vcard _vcard;

        public VCard(Vcard vcard)
        {
            _vcard = vcard;
        }

        public DateTime Birthday
        {
            get
            {
                return _vcard.Birthday;
            }

            set
            {
                _vcard.Birthday = value;
            }
        }

        public string Description
        {
            get
            {
                return _vcard.Description;
            }

            set
            {
                _vcard.Description = value;
            }
        }

        public string Email
        {
            get
            {
                Email email = _vcard.GetPreferedEmailAddress();

                if (email != null)
                {
                    return email.UserId;
                }

                return null;
            }

            set
            {
                Email email = _vcard.GetPreferedEmailAddress();

                if (email == null)
                {
                    _vcard.AddEmailAddress(new Email(EmailType.INTERNET, value, true));
                }
                else
                {
                    email.UserId = value;
                }
            }
        }

        public string Url
        {
            get
            {
                return _vcard.Url;
            }

            set
            {
                _vcard.Url = value;
            }
        }

        public string Phone
        {
            get
            {
                Telephone[] phones = _vcard.GetTelephoneNumbers();

                if (phones != null && phones.Length > 0)
                {
                    return phones[0].Number;
                }

                return null;
            }

            set
            {
                Telephone[] phones = _vcard.GetTelephoneNumbers();

                if (phones == null || phones.Length == 0)
                {
                    _vcard.AddTelephoneNumber(new Telephone(TelephoneLocation.NONE, TelephoneType.NONE, value));
                }
                else
                {
                    phones[0].Number = value;
                }
            }
        }

        public Jid Jid
        {
            get
            {
                return _vcard.JabberId;
            }
        }

        public string NickName
        {
            get
            {
                return _vcard.Nickname;
            }

            set
            {
                _vcard.Nickname = value;
            }
        }

        public string Organization
        {
            get
            {
                Organization organization = _vcard.Organization;

                if (organization != null)
                {
                    return organization.Name;
                }

                return null;
            }

            set
            {
                Organization organization = _vcard.Organization;

                if (organization == null)
                {
                    _vcard.Organization = new Organization(value, string.Empty);
                }
                else
                {
                    _vcard.Organization.Name = value;
                }
            }
        }

        public string Role
        {
            get
            {
                return _vcard.Role;
            }

            set
            {
                _vcard.Role = value;
            }
        }

        public string Title
        {
            get
            {
                return _vcard.Title;
            }

            set
            {
                _vcard.Title = value;
            }
        }
    }
}