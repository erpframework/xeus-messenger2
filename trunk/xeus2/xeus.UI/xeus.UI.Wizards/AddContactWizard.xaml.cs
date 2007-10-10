using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI.xeus.UI.Wizards
{
    /// <summary>
    /// Interaction logic for AddContactWizard.xaml
    /// </summary>
    public partial class AddContactWizard : UserControl
    {
        public AddContactWizard()
        {
            InitializeComponent();

            ICollectionView collectionView = CollectionViewSource.GetDefaultView(_list.ItemsSource);

            collectionView.Filter = FilterRegistered;
        }

        public bool FilterRegistered(object item)
        {
            Service service = item as Service;

            return ((service != null) && service.IsRegistered);
        }
    }
}