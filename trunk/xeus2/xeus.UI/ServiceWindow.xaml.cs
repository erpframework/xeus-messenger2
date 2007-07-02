using System;
using System.Windows;
using System.Windows.Data;
using xeus2.xeus.Commands;

namespace xeus2.xeus.UI
{
    /// <summary>
    /// Interaction logic for ServiceWindow.xaml
    /// </summary>
    public partial class ServiceWindow : Window
    {
        public ServiceWindow()
        {
            InitializeComponent();
        }

        public override void EndInit()
        {
            base.EndInit();

            new TextFilterService(_services.ItemsSource as ListCollectionView, _filter);

            ServiceCommands.RegisterCommands(this);
        }
    }
}