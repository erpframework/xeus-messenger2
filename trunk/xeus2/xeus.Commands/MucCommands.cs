using System.Windows;
using System.Windows.Input;
using agsXMPP.protocol.x.muc;
using xeus2.xeus.Core;
using xeus2.xeus.Middle;

namespace xeus2.xeus.Commands
{
    public static class MucCommands
    {
        private static RoutedUICommand _changeAccessLevel =
            new RoutedUICommand("Change Access Level", "ChangeAccessLevel", typeof (MucCommands));

        private static RoutedUICommand _changeNick =
            new RoutedUICommand("Change Nickname", "ChangeNickmane", typeof (MucCommands));

        private static RoutedUICommand _options =
            new RoutedUICommand("Display Options", "DisplayOptions", typeof(MucCommands));

        private static RoutedUICommand _kick =
            new RoutedUICommand("Kick", "Kick", typeof (MucCommands));

        private static RoutedUICommand _ban =
            new RoutedUICommand("Ban", "Ban", typeof (MucCommands));

        private static RoutedUICommand _sendPrivateMessage =
            new RoutedUICommand("Send Private Message", "SendPrivateMessage", typeof (MucCommands));

        private static RoutedUICommand _invite =
            new RoutedUICommand("Invite", "Invite", typeof (MucCommands));

        private static RoutedUICommand _sendMessageToAll =
            new RoutedUICommand("Send Message To All", "SendMessageToAll", typeof (MucCommands));

        private static RoutedUICommand _modifySubject =
            new RoutedUICommand("Modify Subject", "ModifySubject", typeof (MucCommands));

        private static RoutedUICommand _grantVoice =
            new RoutedUICommand("Grant Voice", "GrantVoice", typeof (MucCommands));

        private static RoutedUICommand _revokeVoice =
            new RoutedUICommand("Revoke Voice", "RevokeVoice", typeof (MucCommands));

        private static RoutedUICommand _addMucMark =
            new RoutedUICommand("Add MUC Bookmark", "AddMucMark", typeof(MucCommands));

         private static RoutedUICommand _deleteMucMark =
            new RoutedUICommand("Delete MUC Bookmark", "DeleteMucMark", typeof(MucCommands));

       public static RoutedUICommand ChangeStatus
        {
            get
            {
                return _changeAccessLevel;
            }
        }

        public static RoutedUICommand ChangeNick
        {
            get
            {
                return _changeNick;
            }
        }

        public static RoutedUICommand SendPrivateMessage
        {
            get
            {
                return _sendPrivateMessage;
            }
        }

        public static RoutedUICommand Invite
        {
            get
            {
                return _invite;
            }
        }

        public static RoutedUICommand SendMessageToAll
        {
            get
            {
                return _sendMessageToAll;
            }
        }

        public static RoutedUICommand ModifySubject
        {
            get
            {
                return _modifySubject;
            }
        }

        public static RoutedUICommand GrantVoice
        {
            get
            {
                return _grantVoice;
            }
        }

        public static RoutedUICommand RevokeVoice
        {
            get
            {
                return _revokeVoice;
            }
        }

        public static RoutedUICommand Kick
        {
            get
            {
                return _kick;
            }
        }

        public static RoutedUICommand Ban
        {
            get
            {
                return _ban;
            }
        }

        public static RoutedUICommand AddMucMark
        {
            get
            {
                return _addMucMark;
            }
        }

        public static RoutedUICommand DeleteMucMark
        {
            get
            {
                return _deleteMucMark;
            }
        }

        public static RoutedUICommand Options
        {
            get
            {
                return _options;
            }
        }

        public static void RegisterCommands(Window window)
        {
            window.CommandBindings.Add(
                new CommandBinding(_changeAccessLevel, ExecuteChangeAccessLevel, CanExecuteChangeAccessLevel));

            window.CommandBindings.Add(
                new CommandBinding(_changeNick, ExecuteChangeNick, CanExecuteChangeNick));

            window.CommandBindings.Add(
                new CommandBinding(_options, ExecuteOptions, CanExecuteOptions));

            window.CommandBindings.Add(
                new CommandBinding(_kick, ExecuteKick, CanExecuteKick));

            window.CommandBindings.Add(
                new CommandBinding(_ban, ExecuteBan, CanExecuteBan));

            window.CommandBindings.Add(
                new CommandBinding(_sendPrivateMessage, ExecuteSendPrivateMessage, CanExecuteSendPrivateMessage));

            window.CommandBindings.Add(
                new CommandBinding(_invite, ExecuteInvite, CanExecuteInvite));

            window.CommandBindings.Add(
                new CommandBinding(_sendMessageToAll, ExecuteSendMessageToAll, CanExecuteSendMessageToAll));

            window.CommandBindings.Add(
                new CommandBinding(_modifySubject, ExecuteModifySubject, CanExecuteModifySubject));

            window.CommandBindings.Add(
                new CommandBinding(_grantVoice, ExecuteGrantVoice, CanExecuteGrantVoice));

            window.CommandBindings.Add(
                new CommandBinding(_revokeVoice, ExecuteRevokeVoice, CanExecuteRevokeVoice));

            window.CommandBindings.Add(
                new CommandBinding(_addMucMark, ExecuteAddMucMark, CanExecuteAddMucMark));

            window.CommandBindings.Add(
                new CommandBinding(_deleteMucMark, ExecuteDeleteMucMark, CanExecuteDeleteMucMark));
        }

        public static void CanExecuteChangeAccessLevel(object sender, CanExecuteRoutedEventArgs e)
        {
            MucContact mucContact = e.Parameter as MucContact;

            if (mucContact != null)
            {
                //e.CanExecute = (mucContact.Nick == mucContact.MucRoom.Nick);
            }

            e.Handled = true;
        }

        public static void ExecuteChangeAccessLevel(object sender, ExecutedRoutedEventArgs e)
        {
            MucContact mucContact = e.Parameter as MucContact;

            if (mucContact != null)
            {
                //ChangeMucContactNick.Instance.DisplayNick(mucContact.MucRoom);
            }

            e.Handled = true;
        }

        public static void CanExecuteChangeNick(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter is MucContact)
            {
                MucContact mucContact = e.Parameter as MucContact;
                e.CanExecute = (mucContact != null && mucContact.Nick == mucContact.MucRoom.Nick);
            }
            else if (e.Parameter is MucRoom)
            {
                MucRoom mucRoom = e.Parameter as MucRoom;
                e.CanExecute = (mucRoom != null && mucRoom.Nick == mucRoom.Me.Nick);
            }

            e.Handled = true;
        }

        public static void ExecuteChangeNick(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is MucContact)
            {
                MucContact mucContact = e.Parameter as MucContact;
                ChangeMucContactNick.Instance.DisplayNick(mucContact.MucRoom);
            }
            else if (e.Parameter is MucRoom)
            {
                MucRoom mucRoom = e.Parameter as MucRoom;
                ChangeMucContactNick.Instance.DisplayNick(mucRoom);
            }

            e.Handled = true;
        }

        public static void CanExecuteOptions(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter is MucContact)
            {
                MucContact mucContact = e.Parameter as MucContact;
                e.CanExecute = (mucContact != null && mucContact.MucRoom != null);
            }
            else if (e.Parameter is MucRoom)
            {
                MucRoom mucRoom = e.Parameter as MucRoom;
                e.CanExecute = (mucRoom != null);
            }

            e.Handled = true;
        }

        public static void ExecuteOptions(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is MucContact)
            {
                MucContact mucContact = e.Parameter as MucContact;
                MucOptions.Instance.DisplayMucOptions(mucContact.MucRoom);
            }
            else if (e.Parameter is MucRoom)
            {
                MucRoom mucRoom = e.Parameter as MucRoom;
                MucOptions.Instance.DisplayMucOptions(mucRoom);
            }

            e.Handled = true;
        }
        public static void CanExecuteKick(object sender, CanExecuteRoutedEventArgs e)
        {
            MucContact mucContact = e.Parameter as MucContact;

            if (mucContact != null)
            {
                // *** A moderator MUST NOT be able to revoke voice privileges from an admin or owner.
                e.CanExecute = (mucContact.MucRoom.Me.Role == Role.moderator);
            }

            e.Handled = true;
        }

        public static void ExecuteKick(object sender, ExecutedRoutedEventArgs e)
        {
            MucContact mucContact = e.Parameter as MucContact;

            if (mucContact != null)
            {
                KickReason.Instance.Kick(mucContact);
            }

            e.Handled = true;
        }

        public static void CanExecuteBan(object sender, CanExecuteRoutedEventArgs e)
        {
            MucContact mucContact = e.Parameter as MucContact;

            if (mucContact != null)
            {
                e.CanExecute = (mucContact.MucRoom.Me.Affiliation == Affiliation.admin
                                || mucContact.MucRoom.Me.Affiliation == Affiliation.owner);
            }

            e.Handled = true;
        }

        public static void ExecuteBan(object sender, ExecutedRoutedEventArgs e)
        {
            MucContact mucContact = e.Parameter as MucContact;

            if (mucContact != null)
            {
                BanReason.Instance.Ban(mucContact);
            }

            e.Handled = true;
        }

        public static void CanExecuteSendPrivateMessage(object sender, CanExecuteRoutedEventArgs e)
        {
            //Service service = e.Parameter as Service ;

            e.Handled = true;
            //e.CanExecute = ( service != null && service.IsRegistrable ) ;
        }

        public static void ExecuteSendPrivateMessage(object sender, ExecutedRoutedEventArgs e)
        {
            //Service service = e.Parameter as Service ;
            //Account.Instance.GetService( service ) ;

            e.Handled = true;
        }

        public static void CanExecuteInvite(object sender, CanExecuteRoutedEventArgs e)
        {
            //Service service = e.Parameter as Service ;

            e.Handled = true;
            //e.CanExecute = ( service != null && service.IsRegistrable ) ;
        }

        public static void ExecuteInvite(object sender, ExecutedRoutedEventArgs e)
        {
            //Service service = e.Parameter as Service ;
            //Account.Instance.GetService( service ) ;

            e.Handled = true;
        }

        public static void CanExecuteSendMessageToAll(object sender, CanExecuteRoutedEventArgs e)
        {
            //Service service = e.Parameter as Service ;

            e.Handled = true;
            //e.CanExecute = ( service != null && service.IsRegistrable ) ;
        }

        public static void ExecuteSendMessageToAll(object sender, ExecutedRoutedEventArgs e)
        {
            //Service service = e.Parameter as Service ;
            //Account.Instance.GetService( service ) ;

            e.Handled = true;
        }

        public static void CanExecuteModifySubject(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = true;
        }

        public static void ExecuteModifySubject(object sender, ExecutedRoutedEventArgs e)
        {
            MucRoom mucRoom = e.Parameter as MucRoom;

            if (mucRoom != null)
            {
                ChangeMucTopic.Instance.DisplayTopic(mucRoom);
            }

            e.Handled = true;
        }

        public static void CanExecuteGrantVoice(object sender, CanExecuteRoutedEventArgs e)
        {
            //Service service = e.Parameter as Service ;

            e.Handled = true;
            //e.CanExecute = ( service != null && service.IsRegistrable ) ;
        }

        public static void ExecuteGrantVoice(object sender, ExecutedRoutedEventArgs e)
        {
            //Service service = e.Parameter as Service ;
            //Account.Instance.GetService( service ) ;

            e.Handled = true;
        }

        public static void CanExecuteRevokeVoice(object sender, CanExecuteRoutedEventArgs e)
        {
            //Service service = e.Parameter as Service ;

            e.Handled = true;
            //e.CanExecute = ( service != null && service.IsRegistrable ) ;
        }

        public static void ExecuteRevokeVoice(object sender, ExecutedRoutedEventArgs e)
        {
            //Service service = e.Parameter as Service ;
            //Account.Instance.GetService( service ) ;

            e.Handled = true;
        }

        public static void CanExecuteAddMucMark(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter is Service)
            {
                Service service = e.Parameter as Service;
                e.CanExecute = (service != null && service.IsChatRoom);
            }
            else if (e.Parameter is MucRoom)
            {
                e.CanExecute = true;
            }

            e.Handled = true;

        }

        public static void ExecuteAddMucMark(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is Service)
            {
                Service service = e.Parameter as Service;
                MucMarks.Instance.AddBookmark(service);
            }
            else if (e.Parameter is MucRoom)
            {
                MucRoom mucRoom = e.Parameter as MucRoom;
                MucMarks.Instance.AddBookmark(mucRoom);
            }

            e.Handled = true;
        }

        public static void CanExecuteDeleteMucMark(object sender, CanExecuteRoutedEventArgs e)
        {
            MucMark mucMark = e.Parameter as MucMark;

            e.Handled = true;

            e.CanExecute = (mucMark != null);
        }

        public static void ExecuteDeleteMucMark(object sender, ExecutedRoutedEventArgs e)
        {
            MucMark mucMark = e.Parameter as MucMark;

            MucMarks.Instance.DeleteBookmark(mucMark);

            e.Handled = true;
        }
    }
}