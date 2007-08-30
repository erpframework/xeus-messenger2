using System;
using System.Windows;
using xeus2.xeus.Commands;
using xeus2.xeus.Core;
using xeus2.xeus.Middle;

namespace xeus2
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class RosterWindow : Window
    {
        public RosterWindow()
        {
            InitializeComponent();
        }

        public override void EndInit()
        {
            base.EndInit();

            ServiceCommands.RegisterCommands(this);
            AccountCommands.RegisterCommands(this);

            Account.Instance.Open();
        }

        protected override void OnClosed(EventArgs e)
        {
            Account.Instance.Close();

            base.OnClosed(e);
        }
         
    }
}