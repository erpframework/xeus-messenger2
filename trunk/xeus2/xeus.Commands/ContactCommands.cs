using System;
using System.Windows;
using System.Windows.Input;
using xeus2.xeus.Core;

namespace xeus2.xeus.Commands
{
    public static class ContactCommands
    {
        private static readonly RoutedUICommand _displayVCard =
            new RoutedUICommand("Display V-Card", "DisplayVCard", typeof (RoutedUICommand));

        private static readonly RoutedUICommand _publishVCard =
            new RoutedUICommand("Publish V-Card", "PublishVCard", typeof(RoutedUICommand));

        public static RoutedUICommand DisplayVCard
        {
            get
            {
                return _displayVCard;
            }
        }

        public static RoutedUICommand PublishVCard
        {
            get
            {
                return _publishVCard;
            }
        }

        public static void RegisterCommands(Window window)
        {
            window.CommandBindings.Add(
                new CommandBinding(_displayVCard, ExecuteDisplayVCard, CanExecuteDisplayVCard));

            window.CommandBindings.Add(
                new CommandBinding(_publishVCard, ExecutePublishVCard, CanExecutePublishVCard));
        }

        private static void CanExecutePublishVCard(object sender, CanExecuteRoutedEventArgs e)
        {
            VCard card = e.Parameter as VCard;

            e.CanExecute = (card != null && !card.IsReadOnly);
            e.Handled = true;
        }

        private static void ExecutePublishVCard(object sender, ExecutedRoutedEventArgs e)
        {
            VCard card = e.Parameter as VCard;

            if (card != null)
            {
                Account.Instance.Self.PublishVCard();
            }
        }

        private static void CanExecuteDisplayVCard(object sender, CanExecuteRoutedEventArgs e)
        {
            IContact contact = e.Parameter as IContact;

            e.CanExecute = (contact != null && contact.Card != null);
            e.Handled = true;
        }

        private static void ExecuteDisplayVCard(object sender, ExecutedRoutedEventArgs e)
        {
            Middle.VCard.Instance.DisplayVCard((IContact) e.Parameter);
        }
    }
}