using System;
using System.Globalization;
using System.Windows.Data;
using agsXMPP.protocol.client;
using xeus2.Properties;

namespace xeus2.xeus.UI.xeus.UI.Look
{
    public class ConvertContactZoom : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string show = (string) value;

            return ((show == ShowType.chat.ToString()
                    || show == ShowType.NONE.ToString())) ? 1.0 : Settings.Default.UI_Item_Zoom_Away;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("This method should never be called");
        }

        #endregion
    }
}