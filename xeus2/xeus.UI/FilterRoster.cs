﻿using System.ComponentModel;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using xeus2.Properties;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI
{
    internal class FilterRoster
    {
        private readonly ICollectionView _collectionView;

        readonly Timer _refreshTimer = new Timer(500);

        public FilterRoster(ICollectionView collectionView, TextBox searchBox)
        {
            _collectionView = collectionView;

            Settings.Default.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
                                                    {
                                                        if (e.PropertyName == "UI_DisplayOfflineContacts")
                                                        {
                                                            Refresh();
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

                                            bool contains =
                                                contact.SearchLowerText.Contains(searchBox.Text.ToLower());

                                            if (contact.IsAvailable)
                                            {
                                                return contains && true;
                                            }
                                            else
                                            {
                                                return contains && Settings.Default.UI_DisplayOfflineContacts;
                                            }
                                        };

            _refreshTimer.Elapsed += _refreshTimer_Elapsed;
        }

        private delegate void RefreshCallback();

        void _refreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            App.InvokeSafe(App._dispatcherPriority, new RefreshCallback(Refresh));
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