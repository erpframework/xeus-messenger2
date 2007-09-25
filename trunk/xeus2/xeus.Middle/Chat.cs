using System;
using xeus2.xeus.Core;
using xeus2.xeus.UI.xeus.UI.Controls;

namespace xeus2.xeus.Middle
{
    internal class Chat : IMultiWinContainerProvider
    {
        private static readonly Chat _instance = new Chat();
        private readonly object _lock = new object();

        private UI.Chat _chat = null;

        public static Chat Instance
        {
            get
            {
                return _instance;
            }
        }

        #region IMultiWinContainerProvider Members

        public MultiTabControl MultiTabControl
        {
            get
            {
                GetChatWindow();
                _chat.Show();

                return _chat._multi;
            }
        }

        public void ShrinkMainWindow(double points)
        {
            lock (_lock)
            {
                if (_chat != null)
                {
                    _chat.Width += points;
                }
            }
        }

        #endregion

        public void DisplayChat(IContact contact)
        {
            App.InvokeSafe(App._dispatcherPriority,
                           new ChatLoginCallback(DisplayChatInternal), contact);
        }

        protected void DisplayChatInternal(IContact contact)
        {
            GetChatWindow();

            _chat.AddChat(contact);

            if (_chat.TabControl.MultiWindows.Count > 0)
            {
                _chat.Show();
            }
            else
            {
                _chat.Close();
            }

            Notification.DismissChatMessageNotification(contact);
        }

        private void GetChatWindow()
        {
            lock (_lock)
            {
                if (_chat == null || !_chat.IsVisible)
                {
                    if (_chat != null)
                    {
                        _chat.Close();
                    }

                    _chat = new UI.Chat();
                    _chat.Closed += _chat_Closed;
                }
            }
        }

        private void _chat_Closed(object sender, EventArgs e)
        {
            lock (_lock)
            {
                _chat.Closed -= _chat_Closed;
                _chat = null;
            }
        }

        #region Nested type: ChatLoginCallback

        private delegate void ChatLoginCallback(IContact contact);

        #endregion
    }
}