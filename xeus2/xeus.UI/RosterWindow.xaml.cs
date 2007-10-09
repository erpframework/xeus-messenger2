using System;
using System.Windows;
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
            Notification.Initialize();

            InitializeComponent();

            new FilterRoster(_roster.CollectionView, _textFilter._textFilter);

            NotificationTray.Instance.Inititalize(_trayIcon, this);

            _filterDisplay.DataContext = _textFilter._textFilter;

            SizeChanged += RosterWindow_SizeChanged;

            Loaded += RosterWindow_Loaded;
        }

        void RosterWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Vista.IsComposition)
            {
                _splitter1.Visibility = Visibility.Collapsed;
                _splitter2.Visibility = Visibility.Collapsed;

                _footerWrapPanel.Margin = new Thickness(0, 5.0, 0, 0);
                _headerContainer.Margin = new Thickness(0);
            }
            else
            {
                _rosterBorder.BorderBrush = null;
                _rosterBorder.Background = null;
            }
        }

        void RosterWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Vista.MakeVistaFrame(this, (int)_headerContainer.ActualHeight, (int)_footerContainer.ActualHeight);
        }

        public override void EndInit()
        {
            base.EndInit();

            ServiceCommands.RegisterCommands(this);
            AccountCommands.RegisterCommands(CommandBindings);
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

            _trayIcon.Dispose();

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
            e.Handled = true;
        }

        private void HistoryPopup(object sender, RoutedEventArgs e)
        {
            _historyPopup.Child = new HistoryListSelection();
            _historyPopup.IsOpen = true;
            
            e.Handled = true;
        }
    }
}