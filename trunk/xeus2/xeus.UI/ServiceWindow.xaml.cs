using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using xeus2.xeus.Commands;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI
{
    /// <summary>
    /// Interaction logic for ServiceWindow.xaml
    /// </summary>
    public partial class ServiceWindow : BaseWindow
    {
        public const string _keyBase = "ServiceWindow";

        public ServiceWindow() : base(_keyBase, string.Empty)
        {
            InitializeComponent();

            DataContext = Account.Instance;

            if (Core.Services.Instance.Transports.Count == 0
                && ServiceCommands.DiscoveryServices.CanExecute(null, null))
            {
                ServiceCommands.DiscoveryServices.Execute(null, null);
            }
        }

        public override void EndInit()
        {
            base.EndInit();

            new TextFilterService(_servicesResult.ItemsSource as ListCollectionView, _filter);

            new TextFilterMucMark(_mucMarksResult.ItemsSource as ListCollectionView, _filterMucMarks);

            new TextFilterMucRoom(_mucResult.ItemsSource as ListCollectionView, _filterMuc, _checkDispEmpty);
        }

        private void MucMarkDblClick(object sender, MouseEventArgs args)
        {
            ListBox list = sender as ListBox;
            
            if (list != null
                && list.SelectedItems.Count > 0
                && ServiceCommands.JoinMuc.CanExecute(list.SelectedItems[0], list))
            {
                ServiceCommands.JoinMuc.Execute(list.SelectedItems[0], list);
            }
        } 

        private void MucDblClick(object sender, MouseEventArgs args)
        {
            ListView list = sender as ListView;
           
            if (list != null
                && list.SelectedItems.Count > 0
                && ServiceCommands.JoinMuc.CanExecute(list.SelectedItems[0], list))
            {
                ServiceCommands.JoinMuc.Execute(list.SelectedItems[0], list);
            }
        } 

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            Account.Instance.MucMarkManager.SaveBookmarks();
        }
    }
}