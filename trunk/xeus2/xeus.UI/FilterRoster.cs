using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Threading;
using xeus2.Properties;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI
{
    internal class FilterRoster
    {
        private readonly ICollectionView _collectionView;

        private readonly DispatcherTimer _refreshTimer = new DispatcherTimer();

        private bool _displayOffline = Settings.Default.UI_DisplayOfflineContacts;
        private bool _displayServices = Settings.Default.UI_DisplayServices;

        public FilterRoster(ICollectionView collectionView, TextBox searchBox)
        {
            _collectionView = collectionView;
            _refreshTimer.IsEnabled = false;
            _refreshTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);

            Settings.Default.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
                                                    {
                                                        switch (e.PropertyName)
                                                        {
                                                            case "UI_DisplayOfflineContacts":
                                                            case "UI_DisplayServices":
                                                                {
                                                                    _displayOffline =
                                                                        Settings.Default.UI_DisplayOfflineContacts;

                                                                    _displayServices =
                                                                        Settings.Default.UI_DisplayServices;

                                                                    Refresh();
                                                                    break;
                                                                }
                                                        }
                                                    };

            searchBox.TextChanged += delegate
                                         {
                                             _refreshTimer.Stop();
                                             _refreshTimer.Start();
                                         };

            collectionView.Filter = delegate(object obj)
                                        {
                                            IContact contact = obj as IContact;

                                            if (contact == null)
                                            {
                                                return false;
                                            }

                                            if (!_displayServices && contact.IsService)
                                            {
                                                return false;
                                            }

                                            bool contains =
                                                contact.SearchLowerText.Contains(searchBox.Text.ToLower());

                                            if (contact.IsAvailable)
                                            {
                                                return contains;
                                            }
                                            else
                                            {
                                                return _displayOffline && contains;
                                            }
                                        };

            _refreshTimer.Tick += _refreshTimer_Tick;
        }

        private void _refreshTimer_Tick(object sender, EventArgs e)
        {
            _refreshTimer.Stop();

            Refresh();
        }

        private void Refresh()
        {
            if (_collectionView != null)
            {
                _collectionView.Refresh();
            }
        }
    }
}