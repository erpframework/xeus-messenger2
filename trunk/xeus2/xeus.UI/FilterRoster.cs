using System.ComponentModel;
using System.Windows.Controls;
using xeus2.Properties;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI
{
    internal class FilterRoster
    {
        private readonly ICollectionView _collectionView;

        public FilterRoster(ICollectionView collectionView)
        {
            _collectionView = collectionView;

            Settings.Default.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "UI_DisplayOfflineContacts")
                {
                    Refresh();
                }
            };

            collectionView.Filter = delegate(object obj)
                                        {
                                            IContact contact = obj as IContact;

                                            if (contact == null)
                                            {
                                                return false;
                                            }
                                            else if (contact.IsAvailable)
                                            {
                                                return true;
                                            }
                                            else
                                            {
                                                return Settings.Default.UI_DisplayOfflineContacts;
                                            }
                                        };
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