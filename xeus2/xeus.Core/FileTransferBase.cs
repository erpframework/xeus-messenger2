using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using xeus2.xeus.Utilities;

namespace xeus2.xeus.Core
{
    public enum FileTransferMode
    {
        Sending,
        Recieving,
        Undefined
    }

    public enum FileTransferState
    {
        Waiting,
        WaitingForResponse,
        Progress,
        Finished,
        Error,
        Cancelled
    }

    internal class FileTransferBase : NotifyInfoDispatcher, IDisposable
    {
        private static readonly ObservableCollectionDisp<FileTransfer> _fileTransfers =
            new ObservableCollectionDisp<FileTransfer>();

        private readonly FileTransferMode _transferMode = FileTransferMode.Undefined;
        protected long _bytesTransmitted = 0;
        private IContact _contact;
        private string _fileDescription = null;

        protected long _fileLength;
        private string _fileName = null;
        private DateTime _lastProgressUpdate;
        
        protected string _filePath;
        protected FileStream _fileStream;

        protected DateTime _startDateTime;

        protected string _sid;

        private FileTransferState _state = FileTransferState.Waiting;
        private string _transmitted;

        private int _progressPercent = 0;
        private string _rate;
        private string _remaining;

        public FileTransferBase(IContact recipient, string filename)
        {
            _transferMode = FileTransferMode.Sending;
            _contact = recipient;
            _fileName = filename;
            
        }
        
        public FileTransferBase(IContact from)
        {
            _transferMode = FileTransferMode.Recieving;
            _contact = from;
        }

        public string FileName
        {
            get
            {
                return _fileName;
            }

            protected set
            {
                _fileName = value;
            }
        }

        public string FileDescription
        {
            get
            {
                return _fileDescription;
            }

            protected set
            {
                _fileDescription = value;
            }
        }

        public string FileSize
        {
            get
            {
                return HRSize(_fileLength);
            }
        }

        public IContact Contact
        {
            get
            {
                return _contact;
            }

            protected set
            {
                _contact = value;
            }
        }

        public FileTransferMode TransferMode
        {
            get
            {
                return _transferMode;
            }
        }

        public int ProgressPercent
        {
            get
            {
                return _progressPercent;
            }

            protected set
            {
                _progressPercent = value;
                NotifyPropertyChanged("ProgressPercent");
            }
        }

        public string Rate
        {
            get
            {
                return _rate;
            }

            protected set
            {
                _rate = value;
                NotifyPropertyChanged("Rate");
            }
        }

        public string Transmitted
        {
            get
            {
                return _transmitted;
            }
            
            protected set
            {
                _transmitted = value;
                NotifyPropertyChanged("Transmitted");
            }
        }

        public string Remaining
        {
            get
            {
                return _remaining;
            }
            
            protected set
            {
                _remaining = value;
                NotifyPropertyChanged("Remaining");
            }
        }

        public FileTransferState State
        {
            get
            {
                return _state;
            }

            protected set
            {
                if (value == FileTransferState.Cancelled
                    || value == FileTransferState.Error
                    || value == FileTransferState.Finished)
                {
                    Dispose();
                }

                _state = value;

                NotifyPropertyChanged("State");
                WPFUtils.RefreshCommands();
            }
        }

        public static ObservableCollectionDisp<FileTransfer> FileTransfers
        {
            get
            {
                return _fileTransfers;
            }
        }


        protected void UpdateProgress()
        {
            // to udate the progress bar	
            TimeSpan ts = DateTime.Now - _lastProgressUpdate;

            if (ts.Milliseconds >= 250)
            {
                _lastProgressUpdate = DateTime.Now;

                double percent = (double) _bytesTransmitted / (double) _fileLength * 100;

                ProgressPercent = (int) percent;
                Rate = GetHRByteRateString();
                Transmitted = HRSize(_bytesTransmitted);
                Remaining = GetHRRemainingTime();
            }
        }

        private string GetHRRemainingTime()
        {
            float fRemaingTime = 0;
            float fTotalNumberOfBytes = _fileLength;
            float fPartialNumberOfBytes = _bytesTransmitted;
            float fBytesPerSecond = GetBytePerSecond();

            if (fBytesPerSecond != 0)
            {
                fRemaingTime = (fTotalNumberOfBytes - fPartialNumberOfBytes) / fBytesPerSecond;
            }

            TimeSpan ts = TimeSpan.FromSeconds(fRemaingTime);

            return String.Format("{0:00}h {1:00}m {2:00}s",
                                 ts.Hours, ts.Minutes, ts.Seconds);
        }

        private long GetBytePerSecond()
        {
            TimeSpan ts = DateTime.Now - _startDateTime;
            double dBytesPerSecond = _bytesTransmitted / ts.TotalSeconds;

            return (long)dBytesPerSecond;
        }

        private static string HRSize(long lBytes)
        {
            StringBuilder sb = new StringBuilder();
            string strUnits = "Bytes";
            float fAdjusted;

            if (lBytes > 1024)
            {
                if (lBytes < 1024 * 1024)
                {
                    strUnits = "KB";
                    fAdjusted = Convert.ToSingle(lBytes) / 1024;
                }
                else
                {
                    strUnits = "MB";
                    fAdjusted = Convert.ToSingle(lBytes) / 1048576;
                }
                sb.AppendFormat("{0:0.0} {1}", fAdjusted, strUnits);
            }
            else
            {
                fAdjusted = Convert.ToSingle(lBytes);
                sb.AppendFormat("{0:0} {1}", fAdjusted, strUnits);
            }

            return sb.ToString();
        }

        private string GetHRByteRateString()
        {
            TimeSpan ts = DateTime.Now - _startDateTime;

            if (ts.TotalSeconds != 0)
            {
                double dBytesPerSecond = _bytesTransmitted / ts.TotalSeconds;
                long lBytesPerSecond = Convert.ToInt64(dBytesPerSecond);
                return HRSize(lBytesPerSecond) + "/s";
            }
            else
            {
                // to fast to calculate a bitrate (0 seconds)
                return HRSize(0) + "/s";
            }
        }


        #region IDisposable Members

        public void Dispose()
        {
            if (_fileStream != null)
            {
                _fileStream.Close();
                _fileStream.Dispose();
            }
        }

        #endregion

        ~FileTransferBase()
        {
            Dispose();
        }
    }
}
