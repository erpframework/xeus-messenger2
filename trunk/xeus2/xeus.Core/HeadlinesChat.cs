using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using xeus2.Properties;
using xeus2.xeus.Data;
using xeus2.xeus.Utilities;

namespace xeus2.xeus.Core
{
    internal class HeadlinesChat : ChatBase<HeadlineMessage>, IDisposable, IChatState
    {
        private readonly ObservableCollectionDisp<HeadlineMessage> _messages =
            new ObservableCollectionDisp<HeadlineMessage>();


        public HeadlinesChat()
        {
            _messages.CollectionChanged += _messages_CollectionChanged;
        }

        public override ObservableCollectionDisp<HeadlineMessage> Messages
        {
            get
            {
                return _messages;
            }
        }

        private void _messages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

        protected override Block GenerateMessage(HeadlineMessage message, HeadlineMessage previousMessage)
        {
            string sender = (string.IsNullOrEmpty(message.From.User)) ? message.From.ToString() : message.From.User;

            _lastMessage = message;

            LoadBrushes();

            Section groupSection = null;

            if (_chatDocument != null)
            {
                groupSection = _chatDocument.Blocks.LastBlock as Section;
            }

            Paragraph paragraph = new Paragraph();

            paragraph.Padding = new Thickness(0.0, 0.0, 0.0, 0.0);
            paragraph.Margin = new Thickness(0.0, 5.0, 0.0, 5.0);

            bool newSection = (groupSection == null);

            paragraph.Inlines.Add(CreateTimeRect(message));

            if (previousMessage == null
                || !JidUtil.BareEquals(previousMessage.From, message.From)
                || (message.DateTime.DateTime - previousMessage.DateTime.DateTime >
                    TimeSpan.FromMinutes(Settings.Default.UI_GroupMessagesByMinutes)))
            {
                if (sender != null)
                {
                    Bold contactName = new Bold();
                    contactName.Cursor = Cursors.Hand;
                    contactName.Foreground = _contactForeground;
                    contactName.Inlines.Add(sender);

                    paragraph.Inlines.Add(contactName);
                    paragraph.Inlines.Add("  ");
                }
                else
                {
                    paragraph.Inlines.Add(CreateRectangle(_bulbBackground));
                }

                newSection = true;
            }

            string body = message.Body;

            if (sender == null)
            {
                // system message
                paragraph.Foreground = _sysTextBrush;
            }

            if (!string.IsNullOrEmpty(body))
            {
                FormatParagraph(paragraph, body);
            }

            paragraph.DataContext = message;

            if (newSection)
            {
                groupSection = new Section();


                groupSection.Blocks.Add(paragraph);
                groupSection.Margin = new Thickness(3.0, 10.0, 3.0, 0.0);
                groupSection.BorderThickness = new Thickness(0.0, 2.0, 0.0, 0.0);
                groupSection.BorderBrush = _alternativeBackground;
            }

            groupSection.DataContext = sender;
            groupSection.Blocks.Add(paragraph);

            return groupSection;
        }

        #region IDisposable Members

        public void Dispose()
        {
            _messages.CollectionChanged -= _messages_CollectionChanged;
        }

        #endregion
    }
}