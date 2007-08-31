using System.Windows;
using System.Windows.Media;

namespace xeus2.xeus.UI
{
    internal static class StyleManager
    {
        public static Style GetStyle(string style)
        {
            return (Style) App.Current.FindResource(style);
        }

        public static Brush GetBrush(string name)
        {
            return (Brush) App.Current.FindResource(name);
        }

        public static DataTemplate GetDataTemplate(string name)
        {
            return (DataTemplate) App.Current.FindResource(name);
        }
    }
}