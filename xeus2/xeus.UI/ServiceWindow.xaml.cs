using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using xeus.Data;
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

            new TextFilterService(_servicesResult.ItemsSource as ListCollectionView, _filter);

            new TextFilterMucMark(_mucMarksResult.ItemsSource as ListCollectionView, _filterMucMarks);

            new TextFilterMucRoom(_mucResult.ItemsSource as ListCollectionView, _filterMuc, _checkDispEmpty);

            ServiceCommands.RegisterCommands(this);
            MucCommands.RegisterCommands(this);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            Database.SaveMucMarks();
        }
    }
}