using System;
using System.ComponentModel;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Threading;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI
{
    internal class TextFilterService
    {
        Timer _keyTime = new Timer(500);
        private ICollectionView _collectionView;

        private delegate void RefreshCallback();


        public TextFilterService()
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
            App.InvokeSafe(DispatcherPriority.Background,
                            new RefreshCallback(Refresh));
        }

        public TextFilterService(ICollectionView collectionView, TextBox textBox) : this()
        {
            string filterText = String.Empty;

            _collectionView = collectionView;

            collectionView.Filter = delegate(object obj)
                                        {
                                            if ( (obj is ServiceCategory)
                                                || string.IsNullOrEmpty(filterText))
                                            {
                                                return true;
                                            }

                                            Service service = obj as Service;

                                            if (service == null || string.IsNullOrEmpty(service.Name))
                                            {
                                                return false;
                                            }

                                            return
                                                service.Name.IndexOf(filterText, 0, StringComparison.CurrentCultureIgnoreCase) >=
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