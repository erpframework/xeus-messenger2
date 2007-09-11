using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using agsXMPP.protocol.client;
using xeus2.Properties;
using xeus2.xeus.Core;
using xeus2.xeus.UI.xeus.UI.Controls;

namespace xeus2.xeus.Commands
{
    public static class RosterCommands
    {
        private static RoutedUICommand _viewBig =
            new RoutedUICommand("Big Roster Items", "BigRosterItems", typeof(RosterCommands));

        private static RoutedUICommand _viewMedium =
            new RoutedUICommand("Medium Roster Items", "MediumRosterItems", typeof(RosterCommands));
        
        private static RoutedUICommand _viewSmall =
            new RoutedUICommand("Small Roster Items", "SmallRosterItems", typeof(RosterCommands));

        private static RoutedUICommand _goOnline =
            new RoutedUICommand("Go Online", "GoOnline", typeof(RosterCommands));

        private static RoutedUICommand _goAway =
            new RoutedUICommand("Go Away", "GoAway", typeof(RosterCommands));

        private static RoutedUICommand _goXAway =
            new RoutedUICommand("Go XAway", "GoXAway", typeof(RosterCommands));

        private static RoutedUICommand _goDND =
            new RoutedUICommand("Go Do not Disturb", "GoDoNotDisturb", typeof(RosterCommands));

        private static RoutedUICommand _goFreeForChat =
            new RoutedUICommand("Go Free for Chat", "GoFreeForChat", typeof(RosterCommands));

        public static RoutedUICommand ViewBig
        {
            get
            {
                return _viewBig;
            }
        }

        public static RoutedUICommand ViewMedium
        {
            get
            {
                return _viewMedium;
            }
        }

        public static RoutedUICommand ViewSmall
        {
            get
            {
                return _viewSmall;
            }
        }

        public static RoutedUICommand GoOnline
        {
            get
            {
                return _goOnline;
            }
        }

        public static RoutedUICommand GoAway
        {
            get
            {
                return _goAway;
            }
        }

        public static RoutedUICommand GoXAway
        {
            get
            {
                return _goXAway;
            }
        }

        public static RoutedUICommand GoDND
        {
            get
            {
                return _goDND;
            }
        }

        public static RoutedUICommand GoFreeForChat
        {
            get
            {
                return _goFreeForChat;
            }
        }

        public static void RegisterCommands(Window window)
        {
            window.CommandBindings.Add(
                new CommandBinding(_viewBig, ExecuteViewBig, CanExecuteViewBig));

            window.CommandBindings.Add(
                new CommandBinding(_viewMedium, ExecuteViewMedium, CanExecuteViewMedium));

            window.CommandBindings.Add(
                new CommandBinding(_viewSmall, ExecuteViewSmall, CanExecuteViewSmall));

            window.CommandBindings.Add(
                new CommandBinding(_goOnline, ExecuteGoOnline, CanExecuteGoOnline));

            window.CommandBindings.Add(
                new CommandBinding(_goAway, ExecuteGoAway, CanExecuteGoAway));

            window.CommandBindings.Add(
                new CommandBinding(_goXAway, ExecuteGoXAway, CanExecuteGoXAway));

            window.CommandBindings.Add(
                new CommandBinding(_goDND, ExecuteGoDND, CanExecuteGoDND));

            window.CommandBindings.Add(
                new CommandBinding(_goFreeForChat, ExecuteGoFreeForChat, CanExecuteGoFreeForChat));
        }

        private static void CanExecuteGoFreeForChat(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = true;
        }

        private static void ExecuteGoFreeForChat(object sender, ExecutedRoutedEventArgs e)
        {
            Account.Instance.SendMyPresence(ShowType.chat);
        }

        private static void CanExecuteGoDND(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = true;
        }

        private static void ExecuteGoDND(object sender, ExecutedRoutedEventArgs e)
        {
            Account.Instance.SendMyPresence(ShowType.dnd);
        }

        private static void CanExecuteGoXAway(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = true;
        }

        private static void ExecuteGoXAway(object sender, ExecutedRoutedEventArgs e)
        {
            Account.Instance.SendMyPresence(ShowType.xa);
        }

        private static void CanExecuteGoAway(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = true;
        }

        private static void ExecuteGoAway(object sender, ExecutedRoutedEventArgs e)
        {
            Account.Instance.SendMyPresence(ShowType.away);
        }

        private static void CanExecuteGoOnline(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = true;
        }

        private static void ExecuteGoOnline(object sender, ExecutedRoutedEventArgs e)
        {
            Account.Instance.SendMyPresence(ShowType.NONE);
        }

        private static void CanExecuteViewSmall(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private static void ExecuteViewSmall(object sender, ExecutedRoutedEventArgs e)
        {
            Settings.Default.UI_RosterItemSize = RosterItemSize.Small;
        }

        private static void CanExecuteViewMedium(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private static void ExecuteViewMedium(object sender, ExecutedRoutedEventArgs e)
        {
            Settings.Default.UI_RosterItemSize = RosterItemSize.Medium;
        }

        private static void CanExecuteViewBig(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private static void ExecuteViewBig(object sender, ExecutedRoutedEventArgs e)
        {
            Settings.Default.UI_RosterItemSize = RosterItemSize.Big;
        }
    }
}
