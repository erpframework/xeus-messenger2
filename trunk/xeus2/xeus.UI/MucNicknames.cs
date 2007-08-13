using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI
{
    internal class MucNikcnames
    {
        private MucRoom _mucRoom;

        List<string> _possibleNicks = new List<string>();
        private List<string>.Enumerator _enumerator = new List<string>.Enumerator();

        private string _before;
        private string _after;

        public MucNikcnames(TextBox textBox, MucRoom mucRoom)
        {
            textBox.PreviewKeyDown += new KeyEventHandler(textBox_KeyDown);

            _mucRoom = mucRoom;
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                TextBox textBox = (TextBox)sender;
                string text = textBox.Text;
                int caretIndex = textBox.CaretIndex;

                if (_enumerator.Current == null)
                {
                    string possibleNick = Utilities.TextUtil.GetWordFromBack(text, caretIndex);

                    _before = text.Substring(0, textBox.CaretIndex - possibleNick.Length);
                    _after = text.Substring(textBox.CaretIndex, text.Length - textBox.CaretIndex);

                    _possibleNicks = GetPossibleNicks(possibleNick);

                    _enumerator = _possibleNicks.GetEnumerator();
                }

                if (!_enumerator.MoveNext())
                {
                    _enumerator = _possibleNicks.GetEnumerator();
                    _enumerator.MoveNext();
                }

                if (_enumerator.Current != null)
                {
                    string delimiter = (_before == String.Empty) ? ": " : " ";

                    textBox.Text = string.Format("{0}{1}{2}{3}", _before, _enumerator.Current, delimiter, _after);
                    textBox.CaretIndex = _before.Length + _enumerator.Current.Length + delimiter.Length;
                }

                e.Handled = true;
            }
            else
            {
                _enumerator = new List<string>.Enumerator();                
            }
        }

        List<string> GetPossibleNicks(string filter)
        {
            List<string> possibleNicks = new List<string>();

            foreach (MucContact mucContact in _mucRoom.MucRoster)
            {
                if (mucContact.Nick.StartsWith(filter, StringComparison.CurrentCultureIgnoreCase))
                {
                    possibleNicks.Add(mucContact.Nick);
                }
            }

            return possibleNicks;
        }
    }
}