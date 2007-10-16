using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using xeus2.xeus.Core;

namespace xeus2.xeus.Commands
{
    public static class GeneralCommands
    {
        private static readonly RoutedUICommand _copyJidToClip =
            new RoutedUICommand("Copy Jid to Clipboard", "CopyJidToClipboard", typeof (RoutedUICommand));

        private static readonly RoutedUICommand _acceptFileTransfer =
            new RoutedUICommand("Accept File", "AcceptFile", typeof(RoutedUICommand));

        private static readonly RoutedUICommand _rejectFileTransfer =
            new RoutedUICommand("Reject File", "RejectFile", typeof(RoutedUICommand));

        private static readonly RoutedUICommand _removeFileTransfer =
            new RoutedUICommand("Remove File Transfer", "RemoveFileTransfer", typeof(RoutedUICommand));

        private static readonly RoutedUICommand _removeFinishedFileTransfers =
            new RoutedUICommand("Remove Finished File Transfer", "RemoveFinishedFileTransfer", typeof(RoutedUICommand));

        private static readonly RoutedUICommand _openFile =
            new RoutedUICommand("Open File", "OpenFile", typeof(RoutedUICommand));

        private static readonly RoutedUICommand _openFileFolder =
            new RoutedUICommand("Open File Location", "OpenFileLocation", typeof(RoutedUICommand));

        public static RoutedUICommand CopyJidToClip
        {
            get
            {
                return _copyJidToClip;
            }
        }

        public static RoutedUICommand AcceptFileTransfer
        {
            get
            {
                return _acceptFileTransfer;
            }
        }

        public static RoutedUICommand RejectFileTransfer
        {
            get
            {
                return _rejectFileTransfer;
            }
        }

        public static RoutedUICommand RemoveFileTransfer
        {
            get
            {
                return _removeFileTransfer;
            }
        }

        public static RoutedUICommand RemoveFinishedFileTransfers
        {
            get
            {
                return _removeFinishedFileTransfers;
            }
        }

        public static RoutedUICommand OpenFile
        {
            get
            {
                return _openFile;
            }
        }

        public static RoutedUICommand OpenFileFolder
        {
            get
            {
                return _openFileFolder;
            }
        }

        public static void RegisterCommands(Window window)
        {
            window.CommandBindings.Add(
                new CommandBinding(_copyJidToClip, ExecuteCopyJidToClip, CanExecuteCopyJidToClip));

            window.CommandBindings.Add(
                new CommandBinding(_acceptFileTransfer, ExecuteAcceptFileTransfer, CanExecuteAcceptFileTransfer));

            window.CommandBindings.Add(
                new CommandBinding(_rejectFileTransfer, ExecuteRejectFileTransfer, CanExecuteRejectFileTransfer));

            window.CommandBindings.Add(
                new CommandBinding(_removeFileTransfer, ExecuteRemoveFileTransfer, CanExecuteRemoveFileTransfer));

            window.CommandBindings.Add(
                new CommandBinding(_removeFinishedFileTransfers, ExecuteRemoveFinishedFileTransfers, CanExecuteRemoveFinishedFileTransfers));

            window.CommandBindings.Add(
                new CommandBinding(_openFile, ExecuteOpenFile, CanExecuteOpenFile));

            window.CommandBindings.Add(
                new CommandBinding(_openFileFolder, ExecuteOpenFileFolder, CanExecuteOpenFileFolder));
        }

        private static void CanExecuteOpenFileFolder(object sender, CanExecuteRoutedEventArgs e)
        {
            FileTransfer fileTransfer = e.Parameter as FileTransfer;

            e.CanExecute = (fileTransfer != null && fileTransfer.State == FileTransferState.Finished);
            e.Handled = true;
        }

        private static void ExecuteOpenFileFolder(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            ((FileTransfer)e.Parameter).OpenFolder();
        }

        private static void CanExecuteOpenFile(object sender, CanExecuteRoutedEventArgs e)
        {
            FileTransfer fileTransfer = e.Parameter as FileTransfer;

            e.CanExecute = (fileTransfer != null && fileTransfer.State == FileTransferState.Finished);
            e.Handled = true;
        }

        private static void ExecuteOpenFile(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            ((FileTransfer)e.Parameter).OpenFile();
        }

        private static void CanExecuteRemoveFinishedFileTransfers(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (FileTransfer.FileTransfers.Count > 0);
            e.Handled = true;
        }

        private static void ExecuteRemoveFinishedFileTransfers(object sender, ExecutedRoutedEventArgs e)
        {
            lock (FileTransfer.FileTransfers._syncObject)
            {
                List<FileTransfer> toBeRemoved = new List<FileTransfer>();

                foreach (FileTransfer transfer in FileTransfer.FileTransfers)
                {
                    if (transfer.State == FileTransferState.Finished
                        || transfer.State == FileTransferState.Cancelled)
                    {
                        toBeRemoved.Add(transfer);
                    }
                }

                foreach (FileTransfer transfer in toBeRemoved)
                {
                    FileTransfer.FileTransfers.Remove(transfer);
                }
            }

            e.Handled = true;
        }

        private static void CanExecuteRemoveFileTransfer(object sender, CanExecuteRoutedEventArgs e)
        {
            FileTransfer fileTransfer = e.Parameter as FileTransfer;

            e.CanExecute = (fileTransfer != null && fileTransfer.State != FileTransferState.Progress);
            e.Handled = true;
        }

        private static void ExecuteRemoveFileTransfer(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            FileTransfer.FileTransfers.Remove((FileTransfer)e.Parameter);
        }

        private static void CanExecuteRejectFileTransfer(object sender, CanExecuteRoutedEventArgs e)
        {
            FileTransfer fileTransfer = e.Parameter as FileTransfer;

            e.CanExecute = (fileTransfer != null && fileTransfer.State == FileTransferState.Waiting);
            e.Handled = true;
        }

        private static void ExecuteRejectFileTransfer(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            ((FileTransfer)e.Parameter).Refuse();
        }

        private static void CanExecuteAcceptFileTransfer(object sender, CanExecuteRoutedEventArgs e)
        {
            FileTransfer fileTransfer = e.Parameter as FileTransfer;

            e.CanExecute = (fileTransfer != null && fileTransfer.State == FileTransferState.Waiting);
            e.Handled = true;
        }

        private static void ExecuteAcceptFileTransfer(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            ((FileTransfer)e.Parameter).Accept();
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