using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.protocol.extensions.bytestreams;
using agsXMPP.protocol.extensions.featureneg;
using agsXMPP.protocol.extensions.si;
using agsXMPP.protocol.x.data;
using MiniClient;
using xeus2.Properties;
using xeus2.xeus.Data;
using File=agsXMPP.protocol.extensions.filetransfer.File;
using Uri=agsXMPP.Uri;

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
        Progress,
        Finished,
        Cancelled
    }

    internal class FileTransfer : NotifyInfoDispatcher
    {
        private static readonly ObservableCollectionDisp<FileTransfer> _fileTransfers = new ObservableCollectionDisp<FileTransfer>();

        #region Mode enum

        #endregion

        private readonly File _file;
        private readonly long _fileLength;
        private readonly SI _si;

        /// <summary>
        /// SID of the filetransfer
        /// </summary>
        private readonly string _sid;

        private readonly FileTransferMode _transferMode = FileTransferMode.Undefined;
        private readonly XmppClientConnection _xmppConnection;
        private long _bytesTransmitted = 0;
        private string _fileDescription = null;
        private string _fileName = null;
        private FileStream _fileStream;
        private IContact _from;
        private DateTime _lastProgressUpdate;
        private int _progressPercent = 0;
        private JEP65Socket _proxySocks5Socket;
        private string _proxyUrl = Settings.Default.XmppBytestreamProxy;
        private string _rate;
        private string _remaining;

        private string _filePath;

        private IQ _siIq;
        private DateTime _startDateTime;
        private IContact _to;

        private FileTransferState _state = FileTransferState.Waiting;

        private string _transmitted;

        public FileTransfer(XmppClientConnection xmppCon, IQ iq, IContact from)
        {
            _transferMode = FileTransferMode.Recieving;

            _siIq = iq;
            _si = iq.SelectSingleElement(typeof (SI)) as SI;

            if (_si != null)
            {
                // get SID for file transfer
                _sid = _si.Id;
                _file = _si.File;
                _from = from;

                From = from;

                if (_file != null)
                {
                    _fileLength = _file.Size;

                    FileDescription = _file.Description;
                    FileName = _file.Name;
                }

                _xmppConnection = xmppCon;
            }
        }

        public string FileName
        {
            get
            {
                return _fileName;
            }
            private set
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
            private set
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

        public IContact From
        {
            get
            {
                return _from;
            }
            private set
            {
                _from = value;
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
            private set
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
            set
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
            set
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
            set
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

            private set
            {
                _state = value;
                NotifyPropertyChanged("State");
            }
        }

        public static ObservableCollectionDisp<FileTransfer> FileTransfers
        {
            get
            {
                return _fileTransfers;
            }
        }

        public void Refuse()
        {
            IQ iq = new IQ();

            iq.To = _siIq.From;
            iq.Id = _siIq.Id;
            iq.Type = IqType.error;

            iq.Error = new Error(ErrorCondition.Forbidden);
            iq.Error.Code = ErrorCode.Forbidden;
            iq.Error.Type = ErrorType.cancel;

            _xmppConnection.Send(iq);

            State = FileTransferState.Cancelled;
        }

        public void Accept()
        {
            FeatureNeg fNeg = _si.FeatureNeg;

            bool ok = false;

            if (fNeg != null)
            {
                agsXMPP.protocol.x.data.Data data = fNeg.Data;
                
                if (data != null)
                {
                    Field[] field = data.GetFields();
                    if (field.Length == 1)
                    {
                        Dictionary<string, string> methods = new Dictionary<string, string>();
                        foreach (Option o in field[0].GetOptions())
                        {
                            string val = o.GetValue();
                            methods.Add(val, val);
                        }

                        if (methods.ContainsKey(Uri.BYTESTREAMS))
                        {
                            // supports bytestream, so choose this option
                            SIIq sIq = new SIIq();
                            sIq.Id = _siIq.Id;
                            sIq.To = _siIq.From;
                            sIq.Type = IqType.result;

                            sIq.SI.Id = _si.Id;
                            sIq.SI.FeatureNeg = new FeatureNeg();

                            agsXMPP.protocol.x.data.Data xdata = new agsXMPP.protocol.x.data.Data();
                            xdata.Type = XDataFormType.submit;
                            Field f = new Field();
                            //f.Type = FieldType.List_Single;
                            f.Var = "stream-method";
                            f.AddValue(Uri.BYTESTREAMS);
                            xdata.AddField(f);
                            sIq.SI.FeatureNeg.Data = xdata;

                            _xmppConnection.OnIq += _xmppConnection_OnIq;
                            _xmppConnection.Send(sIq);

                            ok = true;
                        }
                    }
                }
            }

            if (!ok)
            {
                State = FileTransferState.Cancelled;
            }
        }

        private void _xmppConnection_OnIq(object sender, IQ iq)
        {
            if (iq.Query != null && iq.Query.GetType() == typeof (ByteStream))
            {
                ByteStream bs = iq.Query as ByteStream;

                // check is this is for the correct file
                if (bs != null && bs.Sid == _sid)
                {
                    Thread t = new Thread(
                        delegate()
                            {
                                HandleStreamHost(bs, iq);
                            }
                        );
                    t.Name = "LoopStreamHosts";
                    t.Start();
                }
            }
        }

        private void HandleStreamHost(ByteStream bs, IQ iq)
        {
            //IQ iq = obj as IQ;
            //ByteStream bs = iq.Query as agsXMPP.protocol.extensions.bytestreams.ByteStream;;
            //ByteStream bs = iq.Query as ByteStream;
            if (bs != null)
            {
                _proxySocks5Socket = new JEP65Socket();
                _proxySocks5Socket.OnConnect += m_s5Sock_OnConnect;
                _proxySocks5Socket.OnReceive += m_s5Sock_OnReceive;
                _proxySocks5Socket.OnDisconnect += m_s5Sock_OnDisconnect;

                StreamHost[] streamhosts = bs.GetStreamHosts();
                //Scroll through the possible sock5 servers and try to connect
                //foreach (StreamHost sh in streamhosts)
                //this is done back to front in order to make sure that the proxy JID is chosen first
                //this is necessary as at this stage the application only knows how to connect to a 
                //socks 5 proxy.

                foreach (StreamHost sHost in streamhosts)
                {
                    if (sHost.Host != null)
                    {
                        _proxySocks5Socket.Address = sHost.Host;
                        _proxySocks5Socket.Port = sHost.Port;
                        _proxySocks5Socket.Target = Account.Instance.Self.FullJid;
                        _proxySocks5Socket.Initiator = _from.FullJid;
                        _proxySocks5Socket.SID = _sid;
                        _proxySocks5Socket.ConnectTimeout = 5000;
                        _proxySocks5Socket.SyncConnect();
                        if (_proxySocks5Socket.Connected)
                        {
                            SendStreamHostUsedResponse(sHost, iq);
                            break;
                        }
                    }
                }
            }
        }

        private void m_s5Sock_OnConnect(object sender)
        {
            _startDateTime = DateTime.Now;

            State = FileTransferState.Progress;

            Storage.GetRecievedFolder().Create();

            _filePath = Path.Combine(Storage.GetRecievedFolder().ToString(), _file.Name);
            _fileStream = new FileStream(_filePath, FileMode.Create);
        }

        public void OpenFolder()
        {
            if (!string.IsNullOrEmpty(_filePath))
            {
                Storage.OpenShell(Path.GetDirectoryName(_filePath));
            }
        }

        public void OpenFile()
        {
            if (!string.IsNullOrEmpty(_filePath))
            {
                Storage.OpenShell(_filePath);
            }
        }

        private void SendStreamHostUsedResponse(StreamHost sh, IQ iq)
        {
            ByteStreamIq bsIQ = new ByteStreamIq(IqType.result, _from.FullJid);
            bsIQ.Id = iq.Id;

            bsIQ.Query.StreamHostUsed = new StreamHostUsed(sh.Jid);
            _xmppConnection.Send(bsIQ);
        }

        private void m_s5Sock_OnDisconnect(object sender)
        {
            _fileStream.Close();
            _fileStream.Dispose();

            if (_bytesTransmitted == _fileLength)
            {
                // completed
                // tslTransmitted.Text = "completed";
                // Update Progress when complete
                UpdateProgress();

                State = FileTransferState.Finished;
            }
            else
            {
                State = FileTransferState.Cancelled;
            }
        }

        private void m_s5Sock_OnReceive(object sender, byte[] data, int count)
        {
            _fileStream.Write(data, 0, count);

            _bytesTransmitted += count;

            // to udate the progress bar	
            TimeSpan ts = DateTime.Now - _lastProgressUpdate;

            if (ts.Milliseconds >= 250)
            {
                UpdateProgress();
            }
        }

        private void UpdateProgress()
        {
            _lastProgressUpdate = DateTime.Now;

#pragma warning disable RedundantCast
            double percent = (double)_bytesTransmitted / (double)_fileLength * 100;
#pragma warning restore RedundantCast

            ProgressPercent = (int) percent;
            Rate = GetHRByteRateString();
            Transmitted = HRSize(_bytesTransmitted);
            Remaining = GetHRRemainingTime();
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

            return (long) dBytesPerSecond;
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
    }
}