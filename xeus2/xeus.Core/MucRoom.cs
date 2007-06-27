using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using agsXMPP;
using agsXMPP.Collections;
using agsXMPP.protocol.client;
using xeus2.Properties;
using xeus2.xeus.UI;
using Uri=System.Uri;

namespace xeus2.xeus.Core
{
    internal class MucRoom : NotifyInfoDispatcher, IDisposable
    {
        private MucRoster _mucRoster = new MucRoster();
        private MucMessages _mucMessages = new MucMessages();

        private Service _service;
        private XmppClientConnection _xmppClientConnection = null;
        private readonly string _nick;

        public delegate void MucContactHandler(MucMessage mucMessage);

        public event MucContactHandler OnClickMucContact;

        private FlowDocument _chatDocument = null;

        public MucRoom(Service service, XmppClientConnection xmppClientConnection, string nick)
        {
            _service = service;
            _xmppClientConnection = xmppClientConnection;
            _nick = nick;

            _xmppClientConnection.MesagageGrabber.Add(service.Jid, new BareJidComparer(), new MessageCB(MessageCallback),
                                                      null);
            _xmppClientConnection.PresenceGrabber.Add(service.Jid, new BareJidComparer(),
                                                      new PresenceCB(PresenceCallback),
                                                      null);

            _mucMessages.CollectionChanged += new NotifyCollectionChangedEventHandler(_mucMessages_CollectionChanged);
        }

        private void _mucMessages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Replace:
                    {
                        GenerateChatDocument(e.NewItems);

                        break;
                    }
                case NotifyCollectionChangedAction.Reset:
                    {
                        _chatDocument = null;

                        NotifyPropertyChanged("ChatDocument");

                        break;
                    }
            }
        }

        protected void GenerateChatDocument(IList messages)
        {
            if (_chatDocument == null)
            {
                _chatDocument = new FlowDocument();
                _chatDocument.Foreground = Brushes.Black;
                _chatDocument.FontFamily = new FontFamily("Segoe UI");
                _chatDocument.TextAlignment = TextAlignment.Left;
            }

            foreach (MucMessage message in messages)
            {
                int index = MucMessages.IndexOf(message);

                MucMessage previousMessage = null;

                if (index >= 1)
                {
                    previousMessage = MucMessages[index - 1];
                }

                _chatDocument.Blocks.Add(GenerateMessage(message, previousMessage));
            }

            NotifyPropertyChanged("ChatDocument");
        }

        private static Brush _forMeBackground;
        private static Brush _contactBackground;
        private static Brush _timeBackground;
        //      private static Brush _alternativeForeground;

        private readonly Binding _timeBinding = new Binding("RelativeTime");

        private readonly Regex _urlregex =
            new Regex(
                @"[""'=]?(http://|ftp://|https://|www\.|ftp\.[\w]+)([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public Block GenerateMessage(MucMessage message, MucMessage previousMessage)
        {
            if (_forMeBackground == null)
            {
                _forMeBackground = StyleManager.GetBrush("mymsg_design");
                _contactBackground = StyleManager.GetBrush("mucusername_design");
                _timeBackground = StyleManager.GetBrush("mucmsgtime_design");
            }

            Section groupSection = null;

            if (_chatDocument != null)
            {
                groupSection = _chatDocument.Blocks.LastBlock as Section;
            }

            Paragraph paragraph = new Paragraph();

            paragraph.Padding = new Thickness(0.0, 0.0, 0.0, 0.0);
            paragraph.Margin = new Thickness(0.0, 5.0, 0.0, 10.0);

            bool newSection = (groupSection == null);

            if (previousMessage == null
                || previousMessage.Sender != message.Sender
                ||
                (message.DateTime - previousMessage.DateTime >
                 TimeSpan.FromMinutes(Settings.Default.UI_GroupMessagesByMinutes)))
            {
                /*Image avatar = new Image();
                avatar.Source = message.Image;
                avatar.Width = 30.0;

                paragraph.Inlines.Add(avatar);
                paragraph.Inlines.Add("  ");*/

                if (!string.IsNullOrEmpty(message.Sender))
                {
                    Span contactName = new Span();
                    contactName.Cursor = Cursors.Hand;
                    contactName.Background = _contactBackground;
                    contactName.Inlines.Add(message.Sender);
                    paragraph.Inlines.Add(contactName);
                    paragraph.Inlines.Add("  ");

                    contactName.MouseDown += new MouseButtonEventHandler(contactName_MouseDown);
                }

                newSection = true;
            }

            /*if (message.SentByMe)
            {
                paragraph.Foreground = _alternativeForeground;
            }*/

            if (!string.IsNullOrEmpty(message.Body))
            {
                MatchCollection matches = _urlregex.Matches(message.Body);

                if (matches.Count > 0)
                {
                    string[] founds = new string[matches.Count];

                    for (int i = 0; i < founds.Length; i++)
                    {
                        founds[i] = matches[i].ToString();
                    }

                    string[] bodies = message.Body.Split(founds, StringSplitOptions.RemoveEmptyEntries);

                    for (int j = 0; j < bodies.Length || j < founds.Length; j++)
                    {
                        bool wrongUri = false;

                        if (bodies.Length > j)
                        {
                            paragraph.Inlines.Add(bodies[j]);
                        }

                        if (founds.Length > j)
                        {
                            Run hyperlinkRun = new Run(founds[j]);
                            Hyperlink hyperlink = new XeusHyperlink(hyperlinkRun);
                            hyperlink.Foreground = Brushes.DarkSalmon;

                            try
                            {
                                string url = hyperlinkRun.Text;

                                if (!url.Contains(":"))
                                {
                                    url = string.Format("http://{0}", url);
                                }

                                hyperlink.NavigateUri = new Uri(url);
                            }

                            catch
                            {
                                // improper uri format
                                wrongUri = true;
                            }

                            if (wrongUri)
                            {
                                paragraph.Inlines.Add(hyperlinkRun);
                            }
                            else
                            {
                                paragraph.Inlines.Add(hyperlink);
                            }
                        }
                    }
                }
                else
                {
                    paragraph.Inlines.Add(message.Body);
                }
            }

            paragraph.DataContext = message;

            /*
            TextBlock textBlock = new TextBlock();
            //textBlock.Style = MessageWindow.GetTimeTextBlockStyle();
            textBlock.SetBinding(TextBlock.TextProperty, _timeBinding);

            paragraph.Inlines.Add(textBlock);
            */
            Span time = new Span();
            time.Background = _timeBackground;
            time.FontSize = time.FontSize / 1.3;
            time.Inlines.Add(message.RelativeTime.ToString());
            paragraph.Inlines.Add(time);

            if (newSection)
            {
                groupSection = new Section();

                if (!string.IsNullOrEmpty(message.Body)
                    && message.Body.Contains(_nick))
                {
                    groupSection.Background = _forMeBackground;
                }

                groupSection.Blocks.Add(paragraph);
                groupSection.Margin = new Thickness(3.0, 10.0, 3.0, 0.0);
                groupSection.BorderThickness = new Thickness(0.0, 2.0, 0.0, 0.0);
                //groupSection.BorderBrush = _alternativeBackground;
            }

            groupSection.Blocks.Add(paragraph);

            return groupSection;
        }

        private void contactName_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Span contactSpan = sender as Span;

            if (contactSpan != null)
            {
                MucMessage mucMessage = contactSpan.DataContext as MucMessage;

                if (mucMessage != null && OnClickMucContact != null)
                {
                    OnClickMucContact(mucMessage);

                    e.Handled = true;
                }
            }
        }

        public MucRoster MucRoster
        {
            get
            {
                return _mucRoster;
            }
        }

        public MucMessages MucMessages
        {
            get
            {
                return _mucMessages;
            }
        }

        public Service Service
        {
            get
            {
                return _service;
            }
        }

        public FlowDocument ChatDocument
        {
            get
            {
                return _chatDocument;
            }
        }

        private void MessageCallback(object sender, Message msg, object data)
        {
            if (App.CheckAccessSafe())
            {
                if (msg.Error != null)
                {
                    EventError eventError = new EventError(string.Format("Message error in MUC from {0}", msg.From),
                                                           msg.Error);
                    Events.Instance.OnEvent(this, eventError);
                }
                else
                {
                    MucContact contact;

                    lock (MucRoster._syncObject)
                    {
                        contact = MucRoster.Find(msg.From);
                    }

                    MucMessages.OnMessage(msg, contact);
                }
            }
            else
            {
                App.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                                                   new MessageCB(MessageCallback), sender, msg, data);
            }
        }

        private void PresenceCallback(object sender, Presence presence, object data)
        {
            if (App.CheckAccessSafe())
            {
                if (presence.Error != null)
                {
                    EventError eventError =
                        new EventError(string.Format("Presence error in MUC from {0}", presence.From),
                                       presence.Error);
                    Events.Instance.OnEvent(this, eventError);
                }
                else
                {
                    MucRoster.OnPresence(presence);
                }
            }
            else
            {
                App.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                                                   new PresenceCB(PresenceCallback), sender, presence, data);
            }
        }

        public void SendMessage(string text)
        {
            Message message = new Message();

            message.Type = MessageType.groupchat;
            message.To = _service.Jid;
            message.Body = text;

            _xmppClientConnection.Send(message);
        }

        public void LeaveRoom(string message)
        {
            Presence presence = new Presence();
            presence.To = _service.Jid;
            presence.Type = PresenceType.unavailable;

            if (!string.IsNullOrEmpty(message))
            {
                presence.Status = message;
            }

            _xmppClientConnection.Send(presence);

            _xmppClientConnection.MesagageGrabber.Remove(_service.Jid);
            _xmppClientConnection.PresenceGrabber.Remove(_service.Jid);
        }

        public void Dispose()
        {
            _mucMessages.CollectionChanged -= _mucMessages_CollectionChanged;
        }
    }
}