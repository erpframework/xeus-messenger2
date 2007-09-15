using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    /// <summary>
    /// Interaction logic for SearchText.xaml
    /// </summary>
    public partial class SearchText : UserControl
    {
        private bool _notFound = false;
        private readonly Brush _originalBackground;

        public delegate void ClosedHandler(bool isEnter);

        public event TextChangedEventHandler TextChanged;
        public event ClosedHandler Closed;

        Brush _notFoundBrush = null;

        public SearchText()
        {
            InitializeComponent();

            _text.PreviewKeyDown += _text_KeyDown;
            _originalBackground = _text.Foreground;
        }

        private void _text_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                Close(false);
            }
            else if (e.Key == Key.Return)
            {
                e.Handled = true;
                Close(true);
            }
            else if (e.Key == Key.PageDown
                || e.Key == Key.F3)
            {
                e.Handled = true;
                OnNext(this, null);
            }
        }

        private void OnTextChanged(Object sender, TextChangedEventArgs e)
        {
            if (TextChanged != null)
            {
                TextChanged(sender, e);
            }
        }

        private void Close(bool isEnter)
        {
            Visibility = Visibility.Collapsed;
            _text.Text = String.Empty;

            if (Closed != null)
            {
                Closed(isEnter);
            }
        }

        private void OnCancel(Object sender, RoutedEventArgs e)
        {
            Close(false);
        }

        private void OnNext(Object sender, RoutedEventArgs e)
        {
            SearchNext();
        }

        public bool IsGlobalSearchKey(Key key)
        {
            // ctrl+f, f3
            return ((((ModifierKeys.Control & Keyboard.Modifiers) == ModifierKeys.Control)
                     && (key == Key.F))
                    || (key == Key.F3));
        }

        public bool SendKey(Key key)
        {
            if (Keyboard.Modifiers == 0)
            {
                if ((key >= Key.D0 && key <= Key.Z)
                    || (key >= Key.NumPad0 && key <= Key.NumPad9))
                {
                    Visibility = Visibility.Visible;
                    _text.Focus();

                    return true;
                }
                else if (key == Key.Escape)
                {
                    Close(false);
                    return true;
                }
                else if (key == Key.Return)
                {
                    Close(true);
                    return true;
                }
                else if (key == Key.F3)
                {
                    OnNext(this, null);
                    return true;
                }
            }

            if (((ModifierKeys.Control & Keyboard.Modifiers) == ModifierKeys.Control)
                && (key == Key.F))
            {
                Visibility = Visibility.Visible;
                _text.Focus();

                return true;
            }

            return false;
        }

        public string Text
        {
            get
            {
                return _text.Text;
            }
        }

        public void FocusText()
        {
            _text.Focus();
        }

        public bool NotFound
        {
            get
            {
                return _notFound;
            }
            set
            {
                if (_notFoundBrush == null)
                {
                    _notFoundBrush = StyleManager.GetBrush("notfound_design");
                }

                _notFound = value;
                _text.Foreground = (value) ? _notFoundBrush : _originalBackground;
            }
        }

        public void SearchNext()
        {
            if (TextChanged != null)
            {
                TextChanged(_text, null);
            }
        }
    }
}