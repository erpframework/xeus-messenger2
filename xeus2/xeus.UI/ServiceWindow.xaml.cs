using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using agsXMPP.protocol.x.data;
using xeus.Data;
using xeus2.xeus.Commands;
using xeus2.xeus.Core;

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

        private TextFilterService _textFilterService;
        private TextFilterMucRoom _textFilterMucRoom;

        public override void EndInit()
        {
            base.EndInit();

            new TextFilterService(_servicesResult.ItemsSource as ListCollectionView, _filter);
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