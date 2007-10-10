using System.Windows;
using System.Windows.Controls;

namespace xeus2.xeus.UI.xeus.UI.Zap
{
    public class ZapItemsControl : ItemsControl
    {
        static ZapItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (ZapItemsControl),
                                                     new FrameworkPropertyMetadata(typeof (ZapItemsControl)));
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ContentPresenter;
        }
    }
}