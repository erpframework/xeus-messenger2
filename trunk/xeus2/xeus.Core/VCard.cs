using System;
using System.Windows.Media.Imaging;
using agsXMPP;
using agsXMPP.protocol.iq.vcard;
using agsXMPP.Xml.Dom;
using xeus2.xeus.Data;
using xeus2.xeus.Utilities;

namespace xeus2.xeus.Core
{
    public class VCard
    {
        private readonly Vcard _vcard;
        private readonly Jid _jid;

        public VCard(Vcard vcard, Jid jid)
        {
            _vcard = vcard;
            _jid = jid;
        }

        public bool IsReadOnly
        {
            get
            {
                return !JidUtil.BareEquals(Jid, Account.Instance.Self.Jid);
            }
        }

        public DateTime Birthday
        {
            get
            {
                return Vcard.Birthday;
            }

            set
            {
                Vcard.Birthday = value;
            }
        }

        public string Description
        {
            get
            {
                return Vcard.Description;
            }

            set
            {
                Vcard.Description = value;
            }
        }

        public string Email
        {
            get
            {
                Email email = Vcard.GetPreferedEmailAddress();

                if (email != null)
                {
                    return email.UserId;
                }

                return null;
            }

            set
            {
                Email email = Vcard.GetPreferedEmailAddress();

                if (email == null)
                {
                    Vcard.AddEmailAddress(new Email(EmailType.INTERNET, value, true));
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
                return Vcard.Url;
            }

            set
            {
                Vcard.Url = value;
            }
        }

        public string Phone
        {
            get
            {
                Telephone[] phones = Vcard.GetTelephoneNumbers();

                if (phones != null && phones.Length > 0)
                {
                    return phones[0].Number;
                }

                return null;
            }

            set
            {
                Telephone[] phones = Vcard.GetTelephoneNumbers();

                if (phones == null || phones.Length == 0)
                {
                    Vcard.AddTelephoneNumber(new Telephone(TelephoneLocation.NONE, TelephoneType.NONE, value));
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
                return _jid;
            }
        }

        public string NickName
        {
            get
            {
                return Vcard.Nickname;
            }

            set
            {
                Vcard.Nickname = value;
            }
        }

        public string Organization
        {
            get
            {
                Organization organization = Vcard.Organization;

                if (organization != null)
                {
                    return organization.Name;
                }

                return null;
            }

            set
            {
                Organization organization = Vcard.Organization;

                if (organization == null)
                {
                    Vcard.Organization = new Organization(value, string.Empty);
                }
                else
                {
                    Vcard.Organization.Name = value;
                }
            }
        }

        public string Role
        {
            get
            {
                return Vcard.Role;
            }

            set
            {
                Vcard.Role = value;
            }
        }

        public string Title
        {
            get
            {
                return Vcard.Title;
            }

            set
            {
                Vcard.Title = value;
            }
        }

        public Vcard Vcard
        {
            get
            {
                return _vcard;
            }
        }

        public void SetImage(BitmapImage bitmapImage)
        {
            string base64 = Storage.Base64File(bitmapImage.UriSource.LocalPath);

            if (base64 != null)
            {
                Photo photo = new Photo();
                photo.Type = TextUtil.GetImageType(bitmapImage.UriSource.LocalPath);
                photo.SetTag("BINVAL", base64);

                _vcard.Photo = photo;

                Storage.CacheVCard(_vcard, Jid.Bare);
            }
        }
    }
}