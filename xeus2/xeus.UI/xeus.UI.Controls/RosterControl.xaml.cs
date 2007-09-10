using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

        private Dictionary<string, bool> _expanderStates = new Dictionary<string, bool>();

        public RosterControl()
        {
            Settings.Default.PropertyChanged += Default_PropertyChanged;

            InitializeComponent();

            ItemSize = Settings.Default.UI_RosterItemSize;
        }

        void Default_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "UI_RosterItemSize")
            {
                ItemSize = Settings.Default.UI_RosterItemSize;
            }
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

        private void OnLoadedExpander(object sender, RoutedEventArgs e)
        {
            Expander expander = sender as Expander;
            string expanderName = ((CollectionViewGroup)expander.DataContext).Name.ToString();

            expander.IsExpanded = IsExpanded(expanderName);
        }

        void OnExpanded(object sender, RoutedEventArgs e)
        {
            Expander expander = sender as Expander;
            string expanderName = ((CollectionViewGroup)expander.DataContext).Name.ToString();

            _expanderStates[expanderName] = true;
        }

        void OnCollapsed(object sender, RoutedEventArgs e)
        {
            Expander expander = sender as Expander;
            string expanderName = ((CollectionViewGroup)expander.DataContext).Name.ToString();

            _expanderStates[expanderName] = false;
        }

        bool IsExpanded(string expanderName)
        {
            bool expanded;

            if (_expanderStates.TryGetValue(expanderName, out expanded))
            {
                return expanded;
            }
            else
            {
                _expanderStates[expanderName] = true;
            }

            return true;
        }

    }
}