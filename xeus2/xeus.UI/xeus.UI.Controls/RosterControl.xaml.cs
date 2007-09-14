using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using xeus2.Properties;
using xeus2.xeus.Core;
using xeus2.xeus.Data;

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
        private DataTemplate _dataTemplateSmall = null;
        private DataTemplate _dataTemplateMedium = null;
        private DataTemplate _dataTemplateBig = null;

        private RosterItemSize _rosteritemSize = RosterItemSize.Small;

        private readonly Dictionary<string, bool> _expanderStates = new Dictionary<string, bool>();

        public RosterControl()
        {
            _expanderStates = Database.ReadGroups();

            Settings.Default.PropertyChanged += Default_PropertyChanged;

            InitializeComponent();

            ItemSize = Settings.Default.UI_RosterItemSize;
        }

        public void SaveExpanderState()
        {
            Database.StoreGroups(_expanderStates);
        }

        void Default_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "UI_RosterItemSize")
            {
                ItemSize = Settings.Default.UI_RosterItemSize;
            }
        }

        void ChangeTemplate(DataTemplate dataTemplate)
        {
            _roster.BeginInit();
            _roster.ItemTemplate = dataTemplate;
            _roster.EndInit();
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
                            if (_dataTemplateBig == null)
                            {
                                _dataTemplateBig = StyleManager.GetDataTemplate("MetaContactBig");
                            }

                            ChangeTemplate(_dataTemplateBig);
                            break;
                        }
                    case RosterItemSize.Medium:
                        {
                            if (_dataTemplateMedium == null)
                            {
                                _dataTemplateMedium = StyleManager.GetDataTemplate("MetaContactMedium");
                            }

                            ChangeTemplate(_dataTemplateMedium);
                            break;
                        }
                    default:
                        {
                            if (_dataTemplateSmall == null)
                            {
                                _dataTemplateSmall = StyleManager.GetDataTemplate("MetaContactSmall");
                            }

                            ChangeTemplate(_dataTemplateSmall);
                            break;
                        }
                }
            }
        }

        private void RosterMouseDoubleClick(object sender, RoutedEventArgs args)
        {
            IContact contact = _roster.SelectedItem as IContact;

            if (contact != null)
            {
                Middle.Chat.Instance.DisplayChat(contact);
            }
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

        private void ListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

    }
}