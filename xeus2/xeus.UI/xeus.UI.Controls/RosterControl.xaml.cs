using System.Windows;
using System.Windows.Controls;
using xeus2.Properties;

namespace xeus2.xeus.UI.xeus.UI.Controls
{
    /// <summary>
    /// Interaction logic for RosterControl.xaml
    /// </summary>
    /// 

    public enum RosterItemSize
    {
        Small,
        Medium,
        Big
    }
 
    public partial class RosterControl : UserControl
    {
        private DataTemplate _dataTemplate = null;

        private RosterItemSize _rosteritemSize = RosterItemSize.Small;

        public RosterControl()
        {
            InitializeComponent();

            ItemSize = Settings.Default.UI_RosterItemSize;
        }

        public RosterItemSize ItemSize
        {
            get
            {
                return _rosteritemSize;
            }

            set
            {
                _rosteritemSize = value;

                switch (_rosteritemSize)
                {
                    case RosterItemSize.Big:
                        {
                            _dataTemplate = StyleManager.GetDataTemplate("MetaContactBig");
                            break;
                        }
                    case RosterItemSize.Medium:
                        {
                            _dataTemplate = StyleManager.GetDataTemplate("MetaContactMedium");
                            break;
                        }
                    default:
                        {
                            _dataTemplate = StyleManager.GetDataTemplate("MetaContactSmall");
                            break;
                        }
                }

                BeginInit();
                _roster.ItemTemplate = _dataTemplate;
                EndInit();
            }
        }

        private void RosterMouseDoubleClick(object sender, RoutedEventArgs args)
        {
        }
    }
}