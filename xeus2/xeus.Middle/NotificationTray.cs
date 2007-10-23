using System;
using System.Windows.Forms;
using xeus2.xeus.Core;
using xeus2.xeus.UI;
using xeus2.xeus.UI.xeus.UI.Controls;

namespace xeus2.xeus.Middle
{
    internal class NotificationTray : INotification
    {
        private static readonly NotificationTray _instance = new NotificationTray();

        private TrayIcon _trayIcon = null;
        private BaseWindow _baseWindow = null;

        public static NotificationTray Instance
        {
            get
            {
                return _instance;
            }
        }

        public void Inititalize(TrayIcon trayIcon, BaseWindow baseWindow)
        {
            _baseWindow = baseWindow;
            _trayIcon = trayIcon;
            _trayIcon.NotifyIcon.MouseClick += NotifyIcon_MouseClick;

            _trayIcon.State = TrayIcon.TrayState.Normal;
        }

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            switch (_trayIcon.State)
            {
                case TrayIcon.TrayState.NewMessage:
                    {
                        DismissChatMessageNotification();
                        break;
                    }
                case TrayIcon.TrayState.NewFile:
                    {
                        DismissFileNotification();
                        break;
                    }
                case TrayIcon.TrayState.Normal:
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            _baseWindow.ShowHide();
                        }

                        break;
                    }
            }
        }

        void DismissChatMessageNotification()
        {
            EventChatMessage eventChatMessage = Notification.GetFirstEvent<EventChatMessage>();

            if (eventChatMessage != null)
            {
                Notification.DismissChatMessageNotification(eventChatMessage.Contact);
                Chat.Instance.DisplayChat(eventChatMessage.Contact);
            }
        }

        void DismissFileNotification()
        {
            EventInfoFileTransfer eventInfoFileTransfer = Notification.GetFirstEvent<EventInfoFileTransfer>();

            if (eventInfoFileTransfer != null)
            {
                Notification.DismissNotification(eventInfoFileTransfer);
                FileTransferManager.Instance.TransferOpen(eventInfoFileTransfer.Iq);
            }
        }

        public void RefreshStatus()
        {
            Event lastEvent = Notification.GetFirstEvent<Event>();

            if (lastEvent == null)
            {
                _trayIcon.State = TrayIcon.TrayState.Normal;
                _trayIcon.NotifyIcon.Text = "xeus messenger";
            }
            else if (lastEvent is EventChatMessage)
            {
                _trayIcon.State = TrayIcon.TrayState.NewMessage;
                _trayIcon.NotifyIcon.Text = string.Format("Message from {0}", ((EventChatMessage) lastEvent).Contact.DisplayName);
            }
            else if (lastEvent is EventInfoFileTransfer)
            {
                _trayIcon.State = TrayIcon.TrayState.NewFile;
                _trayIcon.NotifyIcon.Text = lastEvent.Message;
            }
        }
    }
}