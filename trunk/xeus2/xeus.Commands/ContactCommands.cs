using System;
using System.Windows;
using System.Windows.Input;
using agsXMPP;
using xeus2.xeus.Core;
using xeus2.xeus.UI;

namespace xeus2.xeus.Commands
{
    public static class ContactCommands
    {
        private static readonly RoutedUICommand _displayVCard =
            new RoutedUICommand("Display V-Card", "DisplayVCard", typeof (RoutedUICommand));

        private static readonly RoutedUICommand _publishVCard =
            new RoutedUICommand("Publish V-Card", "PublishVCard", typeof(RoutedUICommand));

        private static readonly RoutedUICommand _addContact =
            new RoutedUICommand("Add Contact", "AddContact", typeof(RoutedUICommand));

        private static readonly RoutedUICommand _removeContact =
            new RoutedUICommand("Remove Contact", "RemoveContact", typeof(RoutedUICommand));

        private static readonly RoutedUICommand _displayGroups =
            new RoutedUICommand("Move to Group ...", "DisplayGroups", typeof(RoutedUICommand));

        private static readonly RoutedUICommand _sendFile =
            new RoutedUICommand("Send file", "SendFile", typeof(RoutedUICommand));

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

        public static RoutedUICommand RemoveContact
        {
            get
            {
                return _removeContact;
            }
        }

        public static RoutedUICommand AddContact
        {
            get
            {
                return _addContact;
            }
        }

        public static RoutedUICommand DisplayGroups
        {
            get
            {
                return _displayGroups;
            }
        }

        public static RoutedUICommand SendFile
        {
            get
            {
                return _sendFile;
            }
        }

        public static void RegisterCommands(Window window)
        {
            window.CommandBindings.Add(
                new CommandBinding(_displayVCard, ExecuteDisplayVCard, CanExecuteDisplayVCard));

            window.CommandBindings.Add(
                new CommandBinding(_publishVCard, ExecutePublishVCard, CanExecutePublishVCard));
        
            window.CommandBindings.Add(
                new CommandBinding(_removeContact, ExecuteRemoveContact, CanExecuteRemoveContact));

            window.CommandBindings.Add(
                new CommandBinding(_addContact, ExecuteAddContact, CanExecuteAddContact));

            window.CommandBindings.Add(
                new CommandBinding(_displayGroups, ExecuteDisplayGroups, CanExecuteDisplayGroups));

            window.CommandBindings.Add(
                new CommandBinding(_sendFile, ExecuteSendFile, CanExecuteSendFile));
        }

        private static void CanExecuteSendFile(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (e.Parameter is IContact);
            e.Handled = true;
        }

        private static void ExecuteSendFile(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Middle.FileTransferManager.Instance.SendFile((IContact)e.Parameter);
        }

        private static void CanExecuteDisplayGroups(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (e.Parameter is IContact);
            e.Handled = true;
        }

        private static void ExecuteDisplayGroups(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Middle.Groups.Instance.GroupsOpen((IContact)e.Parameter);
        }

        private static void CanExecuteAddContact(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = (e.Parameter is RegisteredService
                                || e.Parameter is Jid);
        }

        private static void ExecuteAddContact(object sender, ExecutedRoutedEventArgs e)
        {
            RegisteredService service = e.Parameter as RegisteredService;
            e.Handled = true;

            if (service != null)
            {
                Roster.Instance.AddContact(service);
            }
            else
            {
                Jid jid = e.Parameter as Jid;

                if (jid != null)
                {
                    Roster.Instance.AddContact(jid);
                }
            }
        }

        private static void CanExecuteRemoveContact(object sender, CanExecuteRoutedEventArgs e)
        {
            IContact contact = e.Parameter as IContact;

            e.Handled = true;
            e.CanExecute = (contact != null);
        }

        private static void ExecuteRemoveContact(object sender, ExecutedRoutedEventArgs e)
        {
            IContact contact = (IContact)e.Parameter;

            e.Handled = true;

            if (Middle.Alert.Instance.AlertOpenUI(string.Format("Do you really want to remove '{0}'?", contact.DisplayName),
                                              "delete_design", Alert.Buttons.Yes | Alert.Buttons.No) == Alert.Buttons.Yes)
            {
                Roster.Instance.RemoveContact(contact);
            }
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