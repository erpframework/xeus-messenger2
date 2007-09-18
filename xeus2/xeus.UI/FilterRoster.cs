using System.ComponentModel;
using System.Windows.Controls;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI
{
    internal class FilterRoster
    {
        private readonly ICollectionView _collectionView;

        public FilterRoster()
        {
        }

        public FilterRoster(ICollectionView collectionView, CheckBox checkBox)
            : this()
        {
            bool displayOffline = false;

            _collectionView = collectionView;

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
                                                return displayOffline;
                                            }
                                        };

            checkBox.Unchecked += delegate
                                      {
                                          displayOffline = (bool) checkBox.IsChecked;
                                          Refresh();
                                      };

            checkBox.Checked += delegate
                                    {
                                        displayOffline = (bool) checkBox.IsChecked;
                                        Refresh();
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