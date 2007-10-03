using System;
using System.Windows;
using System.Windows.Media.Imaging;
using xeus2.xeus.Commands;
using xeus2.xeus.Core;
using xeus2.xeus.Utilities;

namespace xeus2.xeus.UI
{
    /// <summary>
    /// Interaction logic for VCardDisplay.xaml
    /// </summary>
    public partial class VCardDisplay : BaseWindow
    {
        public const string _keyBase = "VCard";

        public VCardDisplay(IContact contact) : base(_keyBase, contact.Jid.Bare)
        {
            DataContext = contact;

            InitializeComponent();

            ContactCommands.RegisterCommands(this);

            Roster.Instance.SetFreshVcard(contact, 0);

            if (string.IsNullOrEmpty(contact.ClientName))
            {
                Roster.Instance.AskVersion(contact);
            }
        }

        protected void OnDropFile(object sender, DragEventArgs e)
        {
            string[] fileNames = e.Data.GetData(DataFormats.FileDrop, true) as string[];

            if (fileNames != null)
            {
                if (fileNames.Length > 0)
                {
                    string fileName = fileNames[0];

                    if (TextUtil.IsImageExtension(fileName))
                    {
                        // Open a Uri and decode a JPG image
                        Uri uri = new Uri(fileName, UriKind.RelativeOrAbsolute);

                        BitmapImage bitmapImage = new BitmapImage(uri);

                        SelfContact contact = DataContext as SelfContact;

                        if (contact != null)
                        {
                            contact.Image = bitmapImage;
                        }
                    }
                }
            }

            // Mark the event as handled, so the control's native Drop handler is not called.
            e.Handled = true;
        }

        protected void OnDragOver(object sender, DragEventArgs e)
        {
            IContact contact = (IContact) DataContext;

            e.Effects = DragDropEffects.None;

            if (!contact.Card.IsReadOnly)
            {
                string[] fileNames = e.Data.GetData(DataFormats.FileDrop, true) as string[];

                if (fileNames != null)
                {
                    foreach (string fileName in fileNames)
                    {
                        if (TextUtil.IsImageExtension(fileName))
                        {
                            e.Effects = DragDropEffects.Copy;
                        }
                    }
                }
            }

            // Mark the event as handled, so control's native DragOver handler is not called.
            e.Handled = true;
        }
    }
}