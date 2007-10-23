using System;
using System.Collections.Generic;
using System.Text;
using agsXMPP;
using agsXMPP.protocol.client;
using Microsoft.Win32;
using xeus2.xeus.Core;
using xeus2.xeus.UI;

namespace xeus2.xeus.Middle
{
    internal class FileTransferManager
    {
        private static readonly FileTransferManager _instance = new FileTransferManager();

        public static FileTransferManager Instance
        {
            get
            {
                return _instance;
            }
        }

        void TransferOpenUI(IQ iq)
        {
            FileTransfer fileTransfer = new FileTransfer(Account.Instance.XmppConnection, iq,
                                                            Roster.Instance.FindContactOrGetNew(iq.From));
            FileTransfer.FileTransfers.Add(fileTransfer);

            try
            {
                FileTransferWindow fileTransferWindow = new FileTransferWindow();

                fileTransferWindow.Show();
            }

            catch (WindowExistsException e)
            {
                e.ActivateControl();
            }
        }

        void TransferOpenUI(IContact contact, string filename)
        {
            FileTransfer fileTransfer = new FileTransfer(Account.Instance.XmppConnection, contact, filename);
            FileTransfer.FileTransfers.Add(fileTransfer);

            try
            {
                FileTransferWindow fileTransferWindow = new FileTransferWindow();

                fileTransferWindow.Show();
            }

            catch (WindowExistsException e)
            {
                e.ActivateControl();
            }
        }

        public void TransferOpen(IQ iq)
        {
            App.InvokeSafe(App._dispatcherPriority,
                           new DisplayCallback(TransferOpenUI), iq);
        }

        #region Nested type: DisplayCallback

        private delegate void DisplayCallback(IQ iq);

        #endregion

        private delegate void SendCallback(IContact contact);

        public void SendFile(IContact contact)
        {
            App.InvokeSafe(App._dispatcherPriority,
                           new SendCallback(SendFileInternal), contact);
        }

        void SendFileInternal(IContact contact)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".*";
            dlg.Filter = "All files (.*)|*.*";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                TransferOpenUI(contact, dlg.FileName);
            }            
        }
    }
}
