using System.Windows;
using System.Windows.Input;
using xeus2.xeus.Core;

namespace xeus2.xeus.Commands
{
    public static class GeneralCommands
    {
        private static readonly RoutedUICommand _copyJidToClip =
            new RoutedUICommand("Copy Jid to Clipboard", "CopyJidToClipboard", typeof (RoutedUICommand));

        public static RoutedUICommand CopyJidToClip
        {
            get
            {
                return _copyJidToClip;
            }
        }

        public static void RegisterCommands(Window window)
        {
            window.CommandBindings.Add(
                new CommandBinding(_copyJidToClip, ExecuteCopyJidToClip, CanExecuteCopyJidToClip));
        }

        private static void CanExecuteCopyJidToClip(object sender, CanExecuteRoutedEventArgs e)
        {
            IJid jid = e.Parameter as IJid;

            if (jid != null)
            {
                e.CanExecute = (jid.Jid != null);
            }
        }

        private static void ExecuteCopyJidToClip(object sender, ExecutedRoutedEventArgs e)
        {
            IJid jid = e.Parameter as IJid;
            e.Handled = true;

            if (jid != null && jid.Jid != null)
            {
                Clipboard.SetText(jid.Jid.ToString());
            }
        }
    }
}