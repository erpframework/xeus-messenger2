using System;
using System.ComponentModel;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Threading;
using agsXMPP.protocol.x.data;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI
{
    internal class TextFilterMucRoom
    {
        readonly Timer _keyTime = new Timer(500);
        private readonly ICollectionView _collectionView;

        private delegate void RefreshCallback();

        public TextFilterMucRoom()
        {
            _keyTime.AutoReset = false;

            _keyTime.Elapsed += _keyTime_Elapsed;
        }

        void Refresh()
        {
            if (_collectionView != null)
            {
                _collectionView.Refresh();
            }
        }

        void _keyTime_Elapsed(object sender, ElapsedEventArgs e)
        {
            App.InvokeSafe(App._dispatcherPriority,
                            new RefreshCallback(Refresh));
        }

        public TextFilterMucRoom(ICollectionView collectionView, SearchBox searchBox, CheckBox checkBox)
            : this(collectionView, searchBox.TextBox, checkBox)
        {
        }

        public TextFilterMucRoom(ICollectionView collectionView, TextBox textBox, CheckBox checkBox)
            : this()
        {
            string filterText = String.Empty;
            bool displayEmpty = false;

            _collectionView = collectionView;

            collectionView.Filter = delegate(object obj)
                                        {
                                            Service service = obj as Service;

                                            if (service == null || string.IsNullOrEmpty(service.Name)
                                                || !service.IsChatRoom)
                                            {
                                                return false;
                                            }

                                            if (service.Name.IndexOf(filterText, 0, StringComparison.CurrentCultureIgnoreCase) >=0)
                                            {
                                                if (displayEmpty)
                                                {
                                                    return true;
                                                }

                                                if (service.MucInfo != null)
                                                {
                                                    return (service.MucInfo.Occupants > 0);
                                                }
                                                else
                                                {
                                                    return true;
                                                }
                                            }

                                            return false;
                                        };

            checkBox.Unchecked += delegate
                                       {
                                           displayEmpty = (bool)checkBox.IsChecked;
                                           _keyTime.Start();
                                       };

            checkBox.Checked += delegate
                                       {
                                           displayEmpty = (bool)checkBox.IsChecked;
                                           _keyTime.Start();
                                       };

            textBox.TextChanged += delegate
                                       {
                                           filterText = textBox.Text;
                                           _keyTime.Start();
                                       };
        }
    }
}