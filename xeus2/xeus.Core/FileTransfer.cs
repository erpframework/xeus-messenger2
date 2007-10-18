using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using agsXMPP;
using agsXMPP.Collections;
using agsXMPP.protocol.client;
using agsXMPP.protocol.extensions.bytestreams;
using agsXMPP.protocol.extensions.featureneg;
using agsXMPP.protocol.extensions.filetransfer;
using agsXMPP.protocol.extensions.si;
using agsXMPP.protocol.x.data;
using MiniClient;
using xeus2.Properties;
using xeus2.xeus.Data;
using xeus2.xeus.Utilities;
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
        WaitingForResponse,
        Progress,
        Finished,
        Error,
        Cancelled
    }

    internal class FileTransfer : NotifyInfoDispatcher, IDisposable
    {
        private static readonly ObservableCollectionDisp<FileTransfer> _fileTransfers =
            new ObservableCollectionDisp<FileTransfer>();

        private readonly File _file;
        private readonly string _proxyUrl = Settings.Default.XmppBytestreamProxy;
        private readonly SI _si;
        private readonly IQ _siIq;

        private readonly FileTransferMode _transferMode = FileTransferMode.Undefined;
        private long _bytesTransmitted = 0;
        private IContact _contact;
        private string _fileDescription = null;
        private long _fileLength;
        private string _fileName = null;
        private string _filePath;
        private FileStream _fileStream;
        private DateTime _lastProgressUpdate;
        private JEP65Socket _p2pSocks5Socket;
        private int _progressPercent = 0;
        private JEP65Socket _proxySocks5Socket;
        private string _rate;
        private string _remaining;

        /// <summary>
        /// SID of the filetransfer
        /// </summary>
        private string _sid;

        private DateTime _startDateTime;

        private FileTransferState _state = FileTransferState.Waiting;
        private StreamHost _streamHostProxy = null;

        private string _transmitted;
        private XmppClientConnection _xmppConnection;

        public FileTransfer(XmppClientConnection xmppCon, IContact recipient, string filename)
        {
            _transferMode = FileTransferMode.Sending;

            _contact = recipient;
            _fileName = filename;
            _xmppConnection = xmppCon;
        }

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
                _contact = from;

                Contact = from;

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

        public IContact Contact
        {
            get
            {
                return _contact;
            }
            private set
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

        #region IDisposable Members

        public void Dispose()
        {
            if (_xmppConnection != null)
            {
                _xmppConnection.OnIq -= _xmppConnection_OnIq;
                _xmppConnection = null;

                if (_fileStream != null)
                {
                    _fileStream.Close();
                    _fileStream.Dispose();
                }

                if (_proxySocks5Socket != null)
                {
                    _proxySocks5Socket.Disconnect();
                }

                if (_p2pSocks5Socket != null)
                {
                    _p2pSocks5Socket.Disconnect();
                }
            }
        }

        #endregion

        ~FileTransfer()
        {
            Dispose();
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
                State = FileTransferState.Error;
                EventErrorFileTransfer transfer = new EventErrorFileTransfer("Error while negotiating file transfer conditions");
                Events.Instance.OnEvent(this, transfer);
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
                _proxySocks5Socket = new JEP65Socket(_fileLength);
                _proxySocks5Socket.ConnectTimeout = ConnectionTimeout;
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
                        _proxySocks5Socket.Initiator = _contact.FullJid;
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
            ByteStreamIq bsIQ = new ByteStreamIq(IqType.result, _contact.FullJid);
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
            double percent = (double) _bytesTransmitted / (double) _fileLength * 100;
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

        private void SendSiIq()
        {
            SIIq iq = new SIIq();
            iq.To = _contact.FullJid;
            iq.Type = IqType.set;

            _fileLength = new FileInfo(_fileName).Length;

            File afile;
            afile = new File(
                Path.GetFileName(_fileName), _fileLength);

            afile.Description = FileDescription;
            afile.Range = new Range();

            FeatureNeg fNeg = new FeatureNeg();

            agsXMPP.protocol.x.data.Data data = new agsXMPP.protocol.x.data.Data(XDataFormType.form);
            Field f = new Field(FieldType.List_Single);
            f.Var = "stream-method";
            f.AddOption().SetValue(Uri.BYTESTREAMS);
            data.AddField(f);

            fNeg.Data = data;

            iq.SI.File = afile;
            iq.SI.FeatureNeg = fNeg;
            iq.SI.Profile = Uri.SI_FILE_TRANSFER;

            _sid = Guid.NewGuid().ToString();
            iq.SI.Id = _sid;

            _xmppConnection.IqGrabber.SendIq(iq, new IqCB(SiIqResult), null);

            State = FileTransferState.WaitingForResponse;
        }

        private void SiIqResult(object sender, IQ iq, object data)
        {
            // Parse the result of the form
            if (iq.Type == IqType.result)
            {
                SI si = iq.SelectSingleElement(typeof (SI)) as SI;
                if (si != null)
                {
                    FeatureNeg fNeg = si.FeatureNeg;
                    if (SelectedByteStream(fNeg))
                    {
                        DiscoProxy();
                    }
                }
            }
            else if (iq.Type == IqType.error)
            {
                Error err = iq.Error;
                if (err != null)
                {
                    switch ((int) err.Code)
                    {
                        case 403:
                            State = FileTransferState.Cancelled;
                            break;
                        default:
                            State = FileTransferState.Cancelled;
                            break;
                    }
                }

                OnTransferFinish(this);
            }
        }

        protected virtual void OnTransferFinish(object sender)
        {
        }

        private void DiscoProxy()
        {
            ByteStreamIq bsIq = new ByteStreamIq();
            bsIq.To = new Jid(_proxyUrl);
            bsIq.Type = IqType.get;

            _xmppConnection.IqGrabber.SendIq(bsIq, new IqCB(OnProxyDiscoResult), null);
        }

        private void OnProxyDiscoResult(object sender, IQ iq, object data)
        {
            _streamHostProxy = null;

            if (iq.Error == null &&
                iq.Type == IqType.result)
            {
                ByteStream byteStream = iq.Query as ByteStream;

                if (byteStream != null
                    && byteStream.GetStreamHosts().Length > 0)
                {
                    _streamHostProxy = byteStream.GetStreamHosts()[0];
                }
            }

            SendStreamHosts();
        }

        private const int ConnectionTimeout = 60000;

        private void SendStreamHosts()
        {
            ByteStreamIq bsIq = new ByteStreamIq();
            bsIq.To = _contact.FullJid;
            bsIq.Type = IqType.set;

            bsIq.Query.Sid = _sid;

            string hostname = Dns.GetHostName();

            IPHostEntry iphe = Dns.GetHostEntry(hostname);

            for (int i = 0; i < iphe.AddressList.Length; i++)
            {
                Console.WriteLine("IP address: {0}", iphe.AddressList[i]);
                //bsIq.Query.AddStreamHost(Account.Instance.Self.FullJid, iphe.AddressList[i].ToString(), 1000);
            }

            if (_streamHostProxy != null)
            {
                bsIq.Query.AddStreamHost(_streamHostProxy);
            }

            _p2pSocks5Socket = new JEP65Socket();
            _p2pSocks5Socket.ConnectTimeout = ConnectionTimeout;
            _p2pSocks5Socket.Initiator = _xmppConnection.MyJID;
            _p2pSocks5Socket.Target = _contact.FullJid;
            _p2pSocks5Socket.SID = _sid;
            _p2pSocks5Socket.OnConnect += _socket_OnConnect;
            _p2pSocks5Socket.OnDisconnect += _socket_OnDisconnect;
            _p2pSocks5Socket.Listen(1000);

            _xmppConnection.IqGrabber.SendIq(bsIq, new IqCB(SendStreamHostsResult), null);
        }

        private void _socket_OnDisconnect(object sender)
        {
        }

        private void _socket_OnConnect(object sender)
        {
        }

        private void SendStreamHostsResult(object sender, IQ iq, object data)
        {
            if (iq.Type == IqType.result)
            {
                ByteStream bs = iq.Query as ByteStream;

                if (bs != null)
                {
                    Jid sh = bs.StreamHostUsed.Jid;

                    if (sh != null)
                    {
                        if (sh.Equals(Account.Instance.Self.FullJid, new FullJidComparer()))
                        {
                            // direct connection
                            SendFile(null);
                        }

                        if (_streamHostProxy != null
                            && sh.Equals(_streamHostProxy.Jid, new FullJidComparer()))
                        {
                            _p2pSocks5Socket = new JEP65Socket();
                            _p2pSocks5Socket.ConnectTimeout = ConnectionTimeout;
                            _p2pSocks5Socket.Address = _streamHostProxy.Host;
                            _p2pSocks5Socket.Port = _streamHostProxy.Port;
                            _p2pSocks5Socket.Target = _contact.FullJid;
                            _p2pSocks5Socket.Initiator = Account.Instance.Self.FullJid;
                            _p2pSocks5Socket.SID = _sid;
                            _p2pSocks5Socket.ConnectTimeout = 5000;
                            _p2pSocks5Socket.SyncConnect();

                            if (_p2pSocks5Socket.Connected)
                            {
                                ActivateBytestream(_streamHostProxy.Jid);
                            }
                        }
                    }
                }
            }
            else
            {
                State = FileTransferState.Cancelled;
            }
        }

        private void SendFile(IAsyncResult ar)
        {
            const int BUFFERSIZE = 1024;
            byte[] buffer = new byte[BUFFERSIZE];
            FileStream fs;

            State = FileTransferState.Progress;

            // AsyncResult is null when we call this function the 1st time
            if (ar == null)
            {
                _startDateTime = DateTime.Now;
                fs = new FileStream(_fileName, FileMode.Open);
            }
            else
            {
                if (_p2pSocks5Socket.Socket.Connected)
                {
                    _p2pSocks5Socket.Socket.EndReceive(ar);
                }

                fs = ar.AsyncState as FileStream;

                // Windows Forms are not Thread Safe, we need to invoke this :(
                // We're not in the UI thread, so we need to call BeginInvoke
                // to udate the progress bar
                TimeSpan ts = DateTime.Now - _lastProgressUpdate;

                if (ts.Milliseconds >= 250)
                {
                    UpdateProgress();
                }
            }

            int len = fs.Read(buffer, 0, BUFFERSIZE);
            _bytesTransmitted += len;

            if (len > 0)
            {
                _p2pSocks5Socket.Socket.BeginSend(buffer, 0, len, SocketFlags.None, SendFile, fs);
            }
            else
            {
                // Update Pogress when finished
                UpdateProgress();

                fs.Close();
                fs.Dispose();

                if (_p2pSocks5Socket != null && _p2pSocks5Socket.Connected)
                {
                    _p2pSocks5Socket.Disconnect();
                }

                State = FileTransferState.Finished;
            }
        }

        private void ActivateBytestream(Jid streamHost)
        {
            ByteStreamIq bsIq = new ByteStreamIq();

            bsIq.To = streamHost;
            bsIq.Type = IqType.set;

            bsIq.Query.Sid = _sid;
            bsIq.Query.Activate = new Activate(_contact.FullJid);

            _xmppConnection.IqGrabber.SendIq(bsIq, new IqCB(ActivateBytestreamResult), null);
        }

        private void ActivateBytestreamResult(object sender, IQ iq, object dat)
        {
            if (iq.Type == IqType.result)
            {
                SendFile(null);
            }
        }

        private bool SelectedByteStream(FeatureNeg fn)
        {
            if (fn != null)
            {
                agsXMPP.protocol.x.data.Data data = fn.Data;
                if (data != null)
                {
                    foreach (Field field in data.GetFields())
                    {
                        if (field != null && field.Var == "stream-method")
                        {
                            if (field.GetValue() == Uri.BYTESTREAMS)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public void Send()
        {
            SendSiIq();
        }
    }
}