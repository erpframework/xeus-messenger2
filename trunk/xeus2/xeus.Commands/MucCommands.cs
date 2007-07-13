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

        public static void RegisterCommands(Window window)
        {
            window.CommandBindings.Add(
                new CommandBinding(_changeAccessLevel, ExecuteChangeAccessLevel, CanExecuteChangeAccessLevel));

            window.CommandBindings.Add(
                new CommandBinding(_changeNick, ExecuteChangeNick, CanExecuteChangeNick));

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
            MucContact mucContact = e.Parameter as MucContact;

            if (mucContact != null)
            {
                e.CanExecute = (mucContact.Nick == mucContact.MucRoom.Nick);
            }

            e.Handled = true;
        }

        public static void ExecuteChangeNick(object sender, ExecutedRoutedEventArgs e)
        {
            MucContact mucContact = e.Parameter as MucContact;

            if (mucContact != null)
            {
                ChangeMucContactNick.Instance.DisplayNick(mucContact.MucRoom);
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
    }
}