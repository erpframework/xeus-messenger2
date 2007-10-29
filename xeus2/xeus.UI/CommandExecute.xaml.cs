using System;
using System.Windows;
using agsXMPP.protocol.x.data;
using xeus2.xeus.Commands;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI
{
    /// <summary>
    /// Interaction logic for CommandExecute.xaml
    /// </summary>
    public partial class CommandExecute : BaseWindow
    {
        public const string _keyBase = "CommandExecute";

        internal CommandExecute(ServiceCommandExecution serviceCommandExecution):base(_keyBase, serviceCommandExecution.Service.Jid.ToString())
        {
            InitializeComponent();

            DataContext = serviceCommandExecution;

            _execute.Setup(serviceCommandExecution);
            serviceCommandExecution.CommandExec = this;
        }

        internal void Redisplay(ServiceCommandExecution serviceCommandExecution)
        {
            _execute.Setup(serviceCommandExecution);
            serviceCommandExecution.CommandExec = this;
        }

        protected void OnClose(object sender, RoutedEventArgs eventArgs)
        {
            Close();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        public agsXMPP.protocol.x.data.Data GetResult()
        {
            return _execute.GetResult();
        }
    }
}