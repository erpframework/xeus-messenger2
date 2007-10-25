using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using xeus2.xeus.Core;
using xeus2.xeus.Middle;
using xeus2.xeus.Utilities;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    /// <summary>
    /// Interaction logic for Headlines.xaml
    /// </summary>
    public partial class Headlines : UserControl
    {
        private delegate void SelectItemCallback(Message item);

        private HeadlinesChat _headlinesChat = null;

        private readonly InlineMethod _inlineMethod = new InlineMethod();
        private HeadlineMessage _lastFoundItem = null;
        private string _lastSearch = String.Empty;
        private List<TextRange> _previousTextRanges = new List<TextRange>();
        private ScrollViewer _scrollViewer = null;
        private List<KeyValuePair<string, HeadlineMessage>> _texts = null;
        private readonly object _textsLock = new object();
        private string _textToSearch = String.Empty;

        public Headlines()
        {
            InitializeComponent();

            Loaded += Headlines_Loaded;
            _inlineSearch.Loaded += _inlineSearch_Loaded;
        }

        void _inlineSearch_Loaded(object sender, RoutedEventArgs e)
        {
            _inlineSearch.Visibility = Visibility.Collapsed;
        }

        private void Headlines_Loaded(object sender, RoutedEventArgs e)
        {
            _headlinesChat = (HeadlinesChat)DataContext;

            PreviewKeyDown += Conversation_PreviewKeyDown;

            _flowViewer.PreviewKeyDown += _flowViewer_PreviewKeyDown;

            _headlinesChat.Messages.CollectionChanged += Messages_CollectionChanged;

            _inlineMethod.Finished += _inlineMethod_Finished;
            _inlineSearch.TextChanged += _inlineSearch_TextChanged;
            _inlineSearch.Closed += _inlineSearch_Closed;

            Notification.NegotiateAddNotification += Notification_NegotiateAddNotification;

            Unloaded += Conversation_Unloaded;

            ScrollToBottom(true);
        }

        void Notification_NegotiateAddNotification(Event myEvent, NegotiateNotification negotiateNotification)
        {
            EventHeadlineMessage messageEvent = myEvent as EventHeadlineMessage;

            if (messageEvent != null)
            {
                // don't notify if reader is open
                negotiateNotification.Raise = false;
            }
        }

        void _flowViewer_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // all from the viewer
            if (_inlineSearch != null)
            {
                _inlineSearch.SendKey(e.Key);
            }
        }

        void Conversation_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (_inlineSearch.Visibility == Visibility.Visible)
                {
                    e.Handled = _inlineSearch.SendKey(e.Key);
                }
                else
                {
                    // Instance.RemoveCurrentTab();
                }
            }
            else if (_inlineSearch != null)
            {
                if (_inlineSearch.IsGlobalSearchKey(e.Key))
                {
                    e.Handled = _inlineSearch.SendKey(e.Key);
                }
            }
        }

        private void _inlineMethod_Finished(object result)
        {
            Message message = (Message)result;
            SelectItem(message);
        }

        private void _inlineSearch_Closed(bool isEnter)
        {
            lock (_textsLock)
            {
                _texts = null;
                CleanSelection();
            }
        }

        private void _inlineSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            _inlineMethod.Go(new InlineParam(SearchInList, _inlineSearch.Text));
        }

        private object SearchInList(ref bool stop, object param)
        {
            lock (_textsLock)
            {
                if (_texts == null)
                {
                    _texts = new List<KeyValuePair<string, HeadlineMessage>>();

                    lock (_headlinesChat.Messages._syncObject)
                    {
                        foreach (HeadlineMessage mucMessage in _headlinesChat.Messages)
                        {
                            if (!string.IsNullOrEmpty(mucMessage.Body))
                            {
                                _texts.Add(
                                    new KeyValuePair<string, HeadlineMessage>(mucMessage.Body.ToUpper().Trim(), mucMessage));
                            }
                        }
                    }
                }
            }

            HeadlineMessage found = null;

            _textToSearch = (string)param;

            string toFound = ((string)param).ToUpper();

            bool searchNext = (_lastSearch == toFound);

            _lastSearch = toFound;

            if (searchNext && _lastFoundItem != null)
            {
                bool fromHere = false;

                foreach (KeyValuePair<string, HeadlineMessage> body in _texts)
                {
                    if (stop)
                    {
                        return null;
                    }

                    if (fromHere && body.Key.Contains(toFound))
                    {
                        found = body.Value;
                        break;
                    }

                    if (_lastFoundItem == body.Value)
                    {
                        fromHere = true;
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<string, HeadlineMessage> body in _texts)
                {
                    if (stop)
                    {
                        return null;
                    }

                    if (((string)param) == String.Empty)
                    {
                        return null;
                    }

                    if (body.Key.Contains(toFound))
                    {
                        found = body.Value;
                        break;
                    }
                }
            }

            _lastFoundItem = found;
            return found;
        }

        private void SelectItem(Message item)
        {
            if (App.CheckAccessSafe())
            {
                _inlineSearch.NotFound = true;

                if (item != null)
                {
                    foreach (Block block in _flowViewer.Document.Blocks)
                    {
                        Section section = block as Section;

                        if (section != null)
                        {
                            foreach (Block sectionBlock in section.Blocks)
                            {
                                if (sectionBlock.DataContext == item)
                                {
                                    sectionBlock.BringIntoView();

                                    SelectText(sectionBlock as Paragraph, _textToSearch);

                                    _inlineSearch.NotFound = false;

                                    break;
                                }
                            }
                        }

                        if (!_inlineSearch.NotFound)
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                App.InvokeSafe(DispatcherPriority.Send, new SelectItemCallback(SelectItem), item);
            }
        }

        private void SelectText(Paragraph paragraph, string text)
        {
            CleanSelection();
            _previousTextRanges = ContactChat.SelectText(paragraph, text);
        }

        private void CleanSelection()
        {
            foreach (TextRange range in _previousTextRanges)
            {
                range.ApplyPropertyValue(Inline.BackgroundProperty, null);
            }

            _previousTextRanges.Clear();
        }

        void ScrollToBottom(bool force)
        {
            if (_scrollViewer == null)
            {
                _scrollViewer = (ScrollViewer)_flowViewer.Template.FindName("PART_ContentHost", _flowViewer);
            }

            if (force || (_scrollViewer.VerticalOffset >= _scrollViewer.ScrollableHeight - 15.0))
            {
                _scrollViewer.ScrollToBottom();
            }
        }

        void Messages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ScrollToBottom(false);
        }

        private void Conversation_Unloaded(object sender, RoutedEventArgs e)
        {
            Notification.NegotiateAddNotification -= Notification_NegotiateAddNotification;
        }
    }
}