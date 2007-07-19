using System;
using System.ComponentModel;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Threading;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI
{
    internal class TextFilterMucMark
    {
        Timer _keyTime = new Timer(500);
        private ICollectionView _collectionView;

        private delegate void RefreshCallback();

        public TextFilterMucMark()
        {
            _keyTime.AutoReset = false;

            _keyTime.Elapsed += new ElapsedEventHandler(_keyTime_Elapsed);
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

        public TextFilterMucMark(ICollectionView collectionView, SearchBox searchBox)
            : this(collectionView, searchBox.TextBox)
        {
        }

        public TextFilterMucMark(ICollectionView collectionView, TextBox textBox)
            : this()
        {
            string filterText = String.Empty;

            _collectionView = collectionView;

            collectionView.Filter = delegate(object obj)
                                        {
                                            MucMark mucMark = obj as MucMark;

                                            if (mucMark == null || string.IsNullOrEmpty(mucMark.Name))
                                            {
                                                return false;
                                            }

                                            return
                                                mucMark.Name.IndexOf(filterText, 0, StringComparison.CurrentCultureIgnoreCase) >=
                                                0;
                                        };

            textBox.TextChanged += delegate
                                       {
                                           filterText = textBox.Text;
                                           _keyTime.Start();
                                       };
        }
    }
}