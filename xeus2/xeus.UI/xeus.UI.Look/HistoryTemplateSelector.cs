using System.Windows;
using System.Windows.Controls;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI.xeus.UI.Look
{
    public class HistoryTemplateSelector : DataTemplateSelector
    {
        private DataTemplate _mucTemplate;
        private DataTemplate _contactTemplate;

        public DataTemplate MucTemplate
        {
            get
            {
                return _mucTemplate;
            }
            set
            {
                _mucTemplate = value;
            }
        }

        public DataTemplate ContactTemplate
        {
            get
            {
                return _contactTemplate;
            }
            set
            {
                _contactTemplate = value;
            }
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is Service)
            {
                return MucTemplate;
            }
            else
            {
                return ContactTemplate;
            }
        }
    }
}
