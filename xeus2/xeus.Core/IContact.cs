using System;
using System.Windows.Media.Imaging;
using agsXMPP.protocol.client;
using agsXMPP.protocol.extensions.caps;
using agsXMPP.protocol.iq.disco;

namespace xeus2.xeus.Core
{
    public interface IContact : IJid
    {
        Presence Presence
        {
            get;
        }

        int Priority
        {
            get;
        }

        string Resource
        {
            get;
        }

        string DisplayName
        {
            get;
        }

        string Group
        {
            get;
        }

        bool IsAvailable
        {
            get;
        }

        string Show
        {
            get;
        }

        string StatusText
        {
            get;
        }

        string XStatusText
        {
            get;
        }

        string FullName
        {
            get;
        }

        string NickName
        {
            get;
        }

        BitmapImage Image
        {
            get;
        }

        bool IsImageTransparent
        {
            get;
        }

        string CustomName
        {
            get;
        }

        bool IsService
        {
            get;
        }

        string ClientVersion
        {
            get;
        }

        VCard Card
        {
            get;
        }

        DateTime? LastOnlineTime
        {
            get;
        }

        bool HasFeature(string feature);
    }
}