using System;
using System.Globalization;
using System.Windows.Data;

namespace xeus2.xeus.UI.xeus.UI.Look
{
    public class EnumToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                object par = Enum.Parse(value.GetType(), (string)parameter);
                return (value.ToString() == par.ToString());
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Enum.Parse(targetType, (string)parameter);
        }

        #endregion
    }
}