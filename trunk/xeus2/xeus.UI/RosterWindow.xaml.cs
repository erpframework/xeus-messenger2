using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using xeus2.xeus.Commands;
using xeus2.xeus.Core;
using xeus2.xeus.Middle;
using xeus2.xeus.UI;
using xeus2.xeus.UI.xeus.UI.Controls;

namespace xeus2
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class RosterWindow : BaseWindow
    {
        public const string _keyBase = "Roster";

        private static readonly TrayIcon _trayIcon = new TrayIcon();

        public RosterWindow() : base(_keyBase, string.Empty)
        {
            InitializeComponent();

            new FilterRoster(_roster.CollectionView);

            UnreadChatMessages.Instance.Clear();

            TrayIcon.NotifyIcon.MouseClick += NotifyIcon_MouseClick;

            TrayIcon.State = TrayIcon.TrayState.Normal;
        }

        internal static TrayIcon TrayIcon
        {
            get
            {
                return _trayIcon;
            }
        }

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            switch (TrayIcon.State)
            {
                case TrayIcon.TrayState.NewMessage:
                    {
                        UnreadChatMessages.Instance.ClickedNotifyIcon();
                        break;
                    }
                case TrayIcon.TrayState.Normal:
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            ShowHide();
                        }

                        break;
                    }
            }
        }

        public override void EndInit()
        {
            base.EndInit();

            ServiceCommands.RegisterCommands(this);
            AccountCommands.RegisterCommands(this);
            RosterCommands.RegisterCommands(this);
            GeneralCommands.RegisterCommands(this);
            ContactCommands.RegisterCommands(this);

            _header.SetSelfContact(Account.Instance.Self);

            Account.Instance.Open();
        }

        protected override void OnClosed(EventArgs e)
        {
            _roster.SaveExpanderState();

            Account.Instance.Close();

            base.OnClosed(e);

            TrayIcon.Dispose();

            WindowManager.CloseAllWindows();
        }

        protected void ChangeRosterSize(object sebnder, RoutedEventArgs e)
        {
            _roster.ItemSize = RosterItemSize.Big;
        }

        private void DisplayPopupRosterSize(object sender, RoutedEventArgs e)
        {
            _rosterSizePopup.Child = new RosterSizeSelection();
            _rosterSizePopup.IsOpen = true;
        }

        private void MucMarkPopup(object sender, RoutedEventArgs e)
        {
            _mucMarksPopup.IsOpen = true;
        }

        private void HistoryPopup(object sender, RoutedEventArgs e)
        {
            _historyPopup.Child = new HistoryListSelection();
            _historyPopup.IsOpen = true;
        }
    }
}