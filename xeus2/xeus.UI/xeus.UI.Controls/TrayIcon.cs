using System;
using System.Collections.Generic;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;
using xeus2.Properties;
using Timer=System.Timers.Timer;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    internal class TrayIcon : IDisposable
    {
        #region TrayState enum

        public enum TrayState
        {
            Normal,
            NewMessage,
            NewFile,
            Pending
        }

        #endregion

        private readonly Queue<Icon> _file = new Queue<Icon>(2);
        private readonly Queue<Icon> _message = new Queue<Icon>(2);
        private readonly Queue<Icon> _normal = new Queue<Icon>(1);

        private readonly NotifyIcon _notifyIcon = new NotifyIcon();

        private readonly Queue<Icon> _pending = new Queue<Icon>(4);

        private readonly Timer _reloadTime = new Timer(500);
        private TrayState _state = TrayState.Normal;

        public TrayIcon()
        {
            _normal.Enqueue(Resources.xeus);

            /*
			_pending.Enqueue( Properties.Resources.xeus1 );
			_pending.Enqueue( Properties.Resources.xeus2 );
			_pending.Enqueue( Properties.Resources.xeus3 );
			_pending.Enqueue( Properties.Resources.xeus4 );
             */

            _file.Enqueue(Resources.cd_rom);
            _file.Enqueue(Resources.message_trans);

            _message.Enqueue(Resources.message);
            _message.Enqueue(Resources.message_trans);

            _notifyIcon.Visible = true;
            _notifyIcon.Text = "xeus";

            _reloadTime.AutoReset = true;
            _reloadTime.Elapsed += _reloadTime_Elapsed;
            _reloadTime.Start();
        }

        public NotifyIcon NotifyIcon
        {
            get
            {
                return _notifyIcon;
            }
        }

        public TrayState State
        {
            get
            {
                return _state;
            }
            set
            {
                if (_state == value)
                {
                    return;
                }

                _state = value;

                if (_state == TrayState.Normal)
                {
                    _notifyIcon.Text = "xeus";
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            _reloadTime.Stop();

            if (_notifyIcon != null)
            {
                _notifyIcon.Dispose();
            }
        }

        #endregion

        object _syncIcons = new object();

        private void _reloadTime_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (_syncIcons)
            {
                switch (_state)
                {
                    case TrayState.Normal:
                        {
                            _notifyIcon.Icon = _normal.Dequeue();
                            _normal.Enqueue(_notifyIcon.Icon);
                            break;
                        }
                    case TrayState.NewMessage:
                        {
                            _notifyIcon.Icon = _message.Dequeue();
                            _message.Enqueue(_notifyIcon.Icon);
                            break;
                        }
                    case TrayState.NewFile:
                        {
                            _notifyIcon.Icon = _file.Dequeue();
                            _file.Enqueue(_notifyIcon.Icon);
                            break;
                        }
                    case TrayState.Pending:
                        {
                            _notifyIcon.Icon = _pending.Dequeue();
                            _pending.Enqueue(_notifyIcon.Icon);
                            break;
                        }
                }
            }
        }
    }
}