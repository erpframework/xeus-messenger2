using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Documents;
using agsXMPP;
using agsXMPP.protocol.extensions.chatstates;
using xeus2.xeus.UI;
using xeus2.xeus.Utilities;
using Brush=System.Windows.Media.Brush;
using Timer=System.Timers.Timer;
using Uri=System.Uri;
using System.Windows.Controls;

namespace xeus2.xeus.Core
{
    internal abstract class ChatBase<T> : NotifyInfoDispatcher where T : MessageBase
    {
        #region Nested type: TimerCallback

        private delegate void TimerCallback();

        #endregion

        protected T _lastMessage = null;

        private bool _displayTime;

        protected static Brush _alternativeBackground;
        protected static Brush _bulbBackground;
        protected static Brush _contactForeground;

        protected static Brush _forMeForegorund;
        protected static Brush _meTextBrush;

        protected static Brush _selectionFindBrush;
        protected static Brush _sysTextBrush;
        protected static Brush _textBrush;
        protected static Brush _textDimBrush;
        protected static Brush _timeOldBackground;
        protected static Brush _timeOlderBackground;
        protected static Brush _timeOldestBackground;
        protected static Brush _timeRecentBackground;

        protected static Brush _eventBan;
        protected static Brush _eventChangedNick;
        protected static Brush _eventJoined;
        protected static Brush _eventKick;
        protected static Brush _eventLeft;

        protected static Brush _ownAvatarBackground;

        protected readonly Regex _urlregex =
            new Regex(
                @"[""'=]?(http://|ftp://|https://|www\.|ftp\.[\w]+)([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

        protected FlowDocument _chatDocument = null;

        protected readonly List<Rectangle> _relativeTimes = new List<Rectangle>();
        protected readonly DateTime _roomStart = DateTime.Now;

        protected Timer _timeTimer;

        protected XmppClientConnection _xmppClientConnection = null;

        private Chatstate _chatState = Chatstate.None;

        public abstract ObservableCollectionDisp<T> Messages
        {
            get;
        }

        public Chatstate ChatState
        {
            get
            {
                return _chatState;
            }
            set
            {
                _chatState = value;
                NotifyPropertyChanged("ChatState");
            }
        }

        protected static Brush GetMessageTimeBrush(MessageBase mucMessage)
        {
            Brush timeBrush;

            switch (mucMessage.DateTime.Oldness)
            {
                case Oldness.Recent:
                    {
                        timeBrush = _timeRecentBackground;
                        break;
                    }
                case Oldness.Older:
                    {
                        timeBrush = _timeOlderBackground;
                        break;
                    }
                case Oldness.Old:
                    {
                        timeBrush = _timeOldBackground;
                        break;
                    }
                default:
                    {
                        timeBrush = _timeOldestBackground;
                        break;
                    }
            }

            return timeBrush;
        }

        protected Rectangle CreateTimeRect(MessageBase message)
        {
            Rectangle timeRectangle = new Rectangle();
            timeRectangle.Fill = GetMessageTimeBrush(message);
            timeRectangle.Width = 16;
            timeRectangle.Height = 16;

            timeRectangle.Margin = new Thickness(-10.0, 2.0, 4.0, 0.0);
            timeRectangle.Cursor = Cursors.Arrow;
            _relativeTimes.Add(timeRectangle);

            return timeRectangle;
        }

        static readonly object _brushLock = new object();

        private static DataTemplate _emoTemplate;

        protected static void LoadBrushes()
        {
            lock (_brushLock)
            {
                if (_forMeForegorund == null)
                {
                    _forMeForegorund = StyleManager.GetBrush("forme_text_design");

                    _textBrush = StyleManager.GetBrush("text_design");
                    _sysTextBrush = StyleManager.GetBrush("sys_text_design");
                    _meTextBrush = StyleManager.GetBrush("me_text_design");
                    _textDimBrush = StyleManager.GetBrush("textdim_design");

                    _alternativeBackground = StyleManager.GetBrush("back_alt");

                    _contactForeground = StyleManager.GetBrush("muc_contact_fore");
                    _bulbBackground = StyleManager.GetBrush("jabber_design");
                    _ownAvatarBackground = StyleManager.GetBrush("aff_none_design");

                    _timeRecentBackground = StyleManager.GetBrush("time_now_design");
                    _timeOlderBackground = StyleManager.GetBrush("time_older_design");
                    _timeOldBackground = StyleManager.GetBrush("time_old_design");
                    _timeOldestBackground = StyleManager.GetBrush("time_oldest_design");

                    _selectionFindBrush = StyleManager.GetBrush("selection_design");

                    _eventBan = StyleManager.GetBrush("aff_outcast_design");
                    _eventKick = StyleManager.GetBrush("event_kicked_muc_design");
                    _eventLeft = StyleManager.GetBrush("event_left_muc_design");
                    _eventJoined = StyleManager.GetBrush("event_joined_muc_design");
                    _eventChangedNick = StyleManager.GetBrush("event_nickchange_muc_design");

                    _emoTemplate = (DataTemplate)App.Current.FindResource("EmoDisplay");
                }
            }
        }

        protected static void InsertAvatar(Paragraph paragraph, BitmapImage bitmapImage)
        {
            Image image = new Image();
            image.Source = bitmapImage;
            image.Width = 24;
            image.Height = 24;
            paragraph.Inlines.Add(image);
        }

        protected abstract Block GenerateMessage(T message, T previousMessage);

        protected void GenerateChatDocument(IList messages)
        {
            GenerateChatDocumentInternal(messages);
        }

        readonly object _generateLock = new object();

        void GenerateChatDocumentInternal(IList messages)
        {
            lock (_generateLock)
            {
                if (_chatDocument == null)
                {
                    _chatDocument = new FlowDocument();
                    _chatDocument.FontFamily = new FontFamily("Segoe UI");
                    _chatDocument.FontSize = 11.0;
                    _chatDocument.TextAlignment = TextAlignment.Left;
                }
                
                foreach (T message in messages)
                {
                    int index = Messages.IndexOf(message);

                    T previousMessage = null;

                    if (index >= 1)
                    {
                        previousMessage = Messages[index - 1];
                    }

                    _chatDocument.Blocks.Add(GenerateMessage(message, previousMessage));
                }

                if (_timeTimer == null)
                {
                    _timeTimer = new Timer(5000.0);
                    _timeTimer.AutoReset = true;
                    _timeTimer.Elapsed += _timeTimer_Elapsed;
                    _timeTimer.Start();
                }

                NotifyPropertyChanged("ChatDocument");
            }
        }

        private void _timeTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            App.InvokeSafe(App._dispatcherPriority,
                           new TimerCallback(OnRelativeTimer));
        }

        private void OnRelativeTimer()
        {
            foreach (Rectangle time in _relativeTimes)
            {
                MessageBase message = (MessageBase)time.DataContext;

                string text = string.Format("{0}\n{1}", message.DateTime.DateTime.Value,
                                                    message.DateTime.RelativeTime);

                Brush brush = GetMessageTimeBrush(message);

                if (time.Fill != brush)
                {
                    time.Fill = brush;
                }

                time.ToolTip = text;
            }
        }

        protected static Rectangle CreateRectangle(Brush brush)
        {
            Rectangle rect = new Rectangle();
            rect.Fill = brush;
            rect.Width = 20;
            rect.Height = 20;

            return rect;
        }

        static void InsertEmos(Paragraph paragraph, string text)
        {
            string[] emotedText = Emoticons.SplitText(text);
            string[] emos = text.Split(emotedText, StringSplitOptions.RemoveEmptyEntries);

            bool startsWithText = false;

            if (emotedText.Length > 0)
            {
                startsWithText = text.StartsWith(emotedText[0]);
            }

            for (int j = 0; j < emotedText.Length || j < emos.Length; j++)
            {
                if (startsWithText && emotedText.Length > j)
                {
                    paragraph.Inlines.Add(emotedText[j]);
                }

                if (emos.Length > j)
                {
                    string emocode = Emoticons.GetEmoCode(emos[j]);

                    if (emocode == null)
                    {
                        paragraph.Inlines.Add(emos[j]);
                    }
                    else
                    {
                        ContentPresenter presenter = new ContentPresenter();
                        presenter.Width = 20;
                        presenter.Height = 20;
                        presenter.Content = Emoticons.GetEmoCode(emos[j].Trim());
                        presenter.ContentTemplate = _emoTemplate;
                        paragraph.Inlines.Add(presenter);
                    }
                }


                if (!startsWithText && emotedText.Length > j)
                {
                    paragraph.Inlines.Add(emotedText[j]);
                }
            }
        }

        protected void FormatParagraph(Paragraph paragraph, string body)
        {
            if (body.TrimStart().StartsWith("/me "))
            {
                // /me
                body = body.Replace("/me ", String.Empty);
                paragraph.Foreground = _meTextBrush;
            }

            MatchCollection matches = _urlregex.Matches(body);

            if (matches.Count > 0)
            {
                string[] founds = new string[matches.Count];

                for (int i = 0; i < founds.Length; i++)
                {
                    founds[i] = matches[i].ToString();
                }

                string[] bodies = body.Split(founds, StringSplitOptions.RemoveEmptyEntries);

                bool startsWithText = false;

                if (bodies.Length > 0)
                {
                    startsWithText = body.StartsWith(bodies[0]);
                }

                for (int j = 0; j < bodies.Length || j < founds.Length; j++)
                {
                    bool wrongUri = false;

                    if (startsWithText && bodies.Length > j)
                    {
                        //paragraph.Inlines.Add(bodies[j]);
                        InsertEmos(paragraph, bodies[j]);
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

                    if (!startsWithText && bodies.Length > j)
                    {
                        InsertEmos(paragraph, bodies[j]);
                    }

                }
            }
            else
            {
                InsertEmos(paragraph, body);
            }
        }

        public FlowDocument ChatDocument
        {
            get
            {
                return _chatDocument;
            }
        }

        public T LastMessage
        {
            get
            {
                return _lastMessage;
            }
        }

        public bool DisplayTime
        {
            get
            {
                return _displayTime;
            }

            set
            {
                _displayTime = value;

                OnRelativeTimer();
            }
        }


        internal static List<TextRange> SelectText(Paragraph paragraph, string text)
        {
            List<TextRange> textRanges = new List<TextRange>();

            for (Inline inline = paragraph.Inlines.FirstInline; inline != null; inline = inline.NextInline)
            {
                Run run;

                Hyperlink hyperlink = inline as Hyperlink;

                if (hyperlink != null)
                {
                    run = hyperlink.Inlines.FirstInline as Run;
                }
                else
                {
                    run = inline as Run;
                }

                if (run != null)
                {
                    int firstStart = 0;

                    while (true)
                    {
                        if (firstStart > run.Text.Length - 1)
                        {
                            break;
                        }

                        int start = run.Text.IndexOf(text, firstStart, StringComparison.InvariantCultureIgnoreCase);
                        int end = start + text.Length;

                        firstStart = start + 1;

                        if (start >= 0)
                        {
                            TextRange textRange;

                            textRange = new TextRange(run.ContentStart.GetPositionAtOffset(start),
                                                      run.ContentStart.GetPositionAtOffset(end));

                            textRange.ApplyPropertyValue(Run.BackgroundProperty, _selectionFindBrush);

                            textRanges.Add(textRange);
                        }

                        else
                        {
                            break;
                        }
                    }
                }
            }

            return textRanges;
        }

        public void CloseCleanup()
        {
            if (_timeTimer != null)
            {
                _timeTimer.Stop();
                _timeTimer.Elapsed -= _timeTimer_Elapsed;

                //_chatDocument.Blocks.Clear();
            }
        }
    }
}
