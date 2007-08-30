using System;
using System.ComponentModel;
using System.Timers;
using System.Windows.Controls;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI
{
    internal class TextFilterMucAffs
    {
        private Timer _keyTime = new Timer(500);
        private ICollectionView[] _collectionViews;

        private delegate void RefreshCallback();

        public TextFilterMucAffs()
        {
            _keyTime.AutoReset = false;

            _keyTime.Elapsed += new ElapsedEventHandler(_keyTime_Elapsed);
        }

        private void Refresh()
        {
            if (_collectionViews != null)
            {
                foreach (ICollectionView collectionView in _collectionViews)
                {
                    collectionView.Refresh();
                }
            }
        }

        private void _keyTime_Elapsed(object sender, ElapsedEventArgs e)
        {
            App.InvokeSafe(App._dispatcherPriority,
                           new RefreshCallback(Refresh));
        }

        public TextFilterMucAffs(ICollectionView[] collectionViews, SearchBox searchBox)
            : this(collectionViews, searchBox.TextBox)
        {
        }

        public TextFilterMucAffs(ICollectionView[] collectionViews, TextBox textBox)
            : this()
        {
            string filterText = String.Empty;

            _collectionViews = collectionViews;

            foreach (ICollectionView collectionView in _collectionViews)
            {
                collectionView.Filter = delegate(object obj)
                                            {
                                                MucAffContact mucAffContact = obj as MucAffContact;

                                                if (mucAffContact == null || string.IsNullOrEmpty(mucAffContact.Jid))
                                                {
                                                    return false;
                                                }

                                                return
                                                    mucAffContact.Jid.IndexOf(filterText, 0, StringComparison.CurrentCultureIgnoreCase) >= 0;
                                            };

                textBox.TextChanged += delegate
                                           {
                                               filterText = textBox.Text;
                                               _keyTime.Start();
                                           };
            }
        }
    }
}