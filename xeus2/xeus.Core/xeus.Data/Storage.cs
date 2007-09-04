using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using agsXMPP.protocol.iq.vcard;
using agsXMPP.Xml.Dom;
using xeus2.xeus.Core;
using xeus2.xeus.Utilities;

namespace xeus.Data
{
    internal static class Storage
    {
        private static readonly string _folder;

        private static BitmapImage _defaultAvatar;
        private static BitmapImage _defaultServiceAvatar;
        private static readonly SHA1Managed _sha1 = new SHA1Managed();

        static Storage()
        {
            string path = Assembly.GetExecutingAssembly().Location;

            FileInfo fileInfo = new FileInfo(path);
            _folder = fileInfo.DirectoryName;
        }

        public static void OpenShellFolder(string path)
        {
            Process proc = new Process();
            proc.EnableRaisingEvents = false;
            proc.StartInfo.FileName = path;
            proc.Start();
        }

        public static DirectoryInfo GetDbFolder()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(_folder + "\\Database");

            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }

            return directoryInfo;
        }

        public static DirectoryInfo GetRecievedFolder()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(_folder + "\\Files");

            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }

            return directoryInfo;
        }

        private static DirectoryInfo GetCacheFolder()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(_folder + "\\Cache");

            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }

            return directoryInfo;
        }

        public static void CacheVCard(Vcard vcard, string jid)
        {
            try
            {
                DirectoryInfo directoryInfo = GetCacheFolder();

                using (
                    FileStream fileStream =
                        new FileStream(string.Format("{0}\\{1:d}", directoryInfo.FullName, jid.GetHashCode()),
                                       FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    using (StreamWriter streamWriter = new StreamWriter(fileStream))
                    {
                        string vcardXml = string.Format("<vCard>{0}</vCard>", vcard.InnerXml);
                        streamWriter.Write(vcardXml);
                    }
                }
            }

            catch (Exception e)
            {
                Events.Instance.OnEvent(null, new EventError(e.Message, null));
            }
        }

        public static Vcard GetVcard(string jid, int daysToExpire)
        {
            Vcard vcard = null;

            try
            {
                DirectoryInfo directoryInfo = GetCacheFolder();
                FileInfo fileInfo = new FileInfo(string.Format("{0}\\{1:d}", directoryInfo.FullName, jid.GetHashCode()));

                if (fileInfo.CreationTime.AddDays(daysToExpire) < DateTime.Now)
                {
                    return null;
                }

                using (
                    FileStream fileStream =
                        new FileStream(fileInfo.ToString(), FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (StreamReader streamReader = new StreamReader(fileStream))
                    {
                        Document doc = new Document();
                        doc.LoadXml(streamReader.ReadToEnd());

                        if (doc.RootElement != null)
                        {
                            vcard = new Vcard();

                            foreach (Node node in doc.RootElement.ChildNodes)
                            {
                                vcard.AddChild(node);
                            }
                        }
                    }
                }
            }

            catch (Exception e)
            {
                Events.Instance.OnEvent(null, new EventError(e.Message, null));
            }

            return vcard;
        }

        private static BitmapImage GetAvatar(string url, ref BitmapImage avatarStorage)
        {
            if (avatarStorage != null)
            {
                return avatarStorage;
            }

            try
            {
                Uri uri = new Uri(url, UriKind.Absolute);

                using (Stream stream = Application.GetResourceStream(uri).Stream)
                {
                    avatarStorage = new BitmapImage();
                    avatarStorage.CacheOption = BitmapCacheOption.OnLoad;
                    avatarStorage.BeginInit();
                    avatarStorage.StreamSource = stream;
                    avatarStorage.EndInit();
                }

                return avatarStorage;
            }

            catch (Exception e)
            {
                Events.Instance.OnEvent(null, new EventError(e.Message, null));
                return null;
            }
        }

        private static BitmapImage GetAvatar(string url)
        {
            BitmapImage avatarStorage = null;

            try
            {
                Uri uri = new Uri(url, UriKind.Absolute);

                using (Stream stream = Application.GetResourceStream(uri).Stream)
                {
                    avatarStorage = new BitmapImage();
                    avatarStorage.CacheOption = BitmapCacheOption.OnLoad;
                    avatarStorage.BeginInit();
                    avatarStorage.StreamSource = stream;
                    avatarStorage.EndInit();
                }
            }

            catch (Exception e)
            {
                Events.Instance.OnEvent(null, new EventError(e.Message, null));
            }

            return avatarStorage;
        }

        public static BitmapImage GetDefaultAvatar()
        {
            return GetAvatar("pack://application:,,,/xeus.UI/xeus.Images/avatar_default.png", ref _defaultAvatar);
        }

        public static BitmapImage GetDefaultServiceImage()
        {
            return
                GetAvatar("pack://application:,,,../xeus.UI/xeus.Images/service_service.png", ref _defaultServiceAvatar);
        }

        public static BitmapImage GetServiceImage(string name)
        {
            return GetAvatar(string.Format("pack://application:,,,../xeus.UI/xeus.Images/{0}", name));
        }

        /*
        public static string FlushImage(string jid)
        {
            try
            {
                Vcard vcard = GetVcard(jid);

                if (vcard == null || vcard.Photo == null)
                {
                    return null;
                }

                string filename = Path.GetTempFileName();
                byte[] pic;

                if (vcard.Photo.HasTag("BINVAL"))
                {
                    pic = Convert.FromBase64String(vcard.Photo.GetTag("BINVAL"));
                }
                else if (vcard.Photo.TextBase64.Length > 0)
                {
                    pic = Convert.FromBase64String(vcard.Photo.Value);
                }
                else
                {
                    return null;
                }

                using (BinaryWriter binWriter = new BinaryWriter(File.Open(filename, FileMode.Create)))
                {
                    binWriter.Write(pic);
                }

                return filename;
            }

            catch
            {
                return null;
            }
        }*/

        public static string GetPhotoHashCode(Photo photo)
        {
            if (photo == null)
            {
                return String.Empty;
            }

            try
            {
                byte[] pic = null;

                if (photo.HasTag("BINVAL"))
                {
                    pic = Convert.FromBase64String(photo.GetTag("BINVAL"));
                }
                else if (photo.TextBase64.Length > 0)
                {
                    pic = Convert.FromBase64String(photo.Value);
                }

                if (pic != null)
                {
                    return TextUtil.HexEncode(_sha1.ComputeHash(pic));
                }
            }

            catch
            {
            }

            return String.Empty;
        }

        public static BitmapImage ImageFromPhoto(Photo photo)
        {
            try
            {
                if (photo == null)
                {
                    return null;
                }
                else if (photo.HasTag("BINVAL"))
                {
                    byte[] pic = Convert.FromBase64String(photo.GetTag("BINVAL"));
                    MemoryStream memoryStream = new MemoryStream(pic, 0, pic.Length);
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = memoryStream;
                    bitmap.EndInit();
                    return bitmap;
                }
                else if (photo.HasTag("EXTVAL"))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri(photo.GetTag("EXTVAL"));
                    bitmap.EndInit();

                    return bitmap;
                }
                else if (photo.TextBase64.Length > 0)
                {
                    byte[] pic = Convert.FromBase64String(photo.Value);
                    MemoryStream memoryStream = new MemoryStream(pic, 0, pic.Length);
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.BeginInit();
                    bitmap.StreamSource = memoryStream;
                    bitmap.EndInit();

                    return bitmap;
                }
                else
                {
                    return null;
                }
            }

            catch
            {
                return null;
            }
        }
    }
}