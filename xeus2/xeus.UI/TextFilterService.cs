using System;
using System.ComponentModel;
using System.Windows.Controls;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI
{
    internal class TextFilterService
    {
        public TextFilterService(ICollectionView collectionView, TextBox textBox)
        {
            string filterText = String.Empty;

            collectionView.Filter = delegate(object obj)
                                        {
                                            if ( (obj is ServiceCategory)
                                                || string.IsNullOrEmpty(filterText))
                                            {
                                                return true;
                                            }

                                            string str = obj as string;

                                            if (string.IsNullOrEmpty(str))
                                            {
                                                return false;
                                            }

                                            return
                                                str.IndexOf(filterText, 0, StringComparison.CurrentCultureIgnoreCase) >=
                                                0;
                                        };

            textBox.TextChanged += delegate
                                       {
                                           filterText = textBox.Text;
                                           collectionView.Refresh();
                                       };
        }
    }
}