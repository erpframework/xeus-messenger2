using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using agsXMPP.protocol.extensions.chatstates;

namespace xeus2.xeus.Core
{
    internal class ChatStateNotificator
    {
        public delegate void ChangeStateHandler(Chatstate chatstate);
        public event ChangeStateHandler StateChanged;

        private delegate void SendChatStateCallback(Chatstate chatstate);

        private readonly Timer _timerNoTyping = new Timer(5000);
        private readonly Timer _timerNoTyping2 = new Timer(20000);
        private Chatstate _chatstate = Chatstate.None;

        public ChatStateNotificator()
        {
            _timerNoTyping.Elapsed += _timerNoTyping_Elapsed;
            _timerNoTyping2.Elapsed += _timerNoTyping2_Elapsed;
        }

        void _timerNoTyping2_Elapsed(object sender, ElapsedEventArgs e)
        {
            ChangeChatState(Chatstate.inactive);
        }

        void _timerNoTyping_Elapsed(object sender, ElapsedEventArgs e)
        {
            ChangeChatState(Chatstate.paused);
        }

        public void ChangeChatState(Chatstate chatstate)
        {
            if (App.CheckAccessSafe())
            {
                if (chatstate == Chatstate.composing)
                {
                    _timerNoTyping.Start();
                    _timerNoTyping2.Stop();
                }

                if (_chatstate == chatstate)
                {
                    return;
                }

                switch (chatstate)
                {
                    case Chatstate.paused:
                        {
                            _timerNoTyping.Stop();
                            _timerNoTyping2.Start();
                            break;
                        }
                    case Chatstate.inactive:
                    case Chatstate.gone:
                        {
                            _timerNoTyping.Stop();
                            _timerNoTyping2.Stop();
                            break;
                        }
                }

                if (StateChanged != null)
                {
                    StateChanged(chatstate);
                }

                _chatstate = chatstate;
            }
            else
            {
               App.InvokeSafe(App._dispatcherPriority,
                                                  new SendChatStateCallback(ChangeChatState), chatstate);
            }
        }
    }
}
