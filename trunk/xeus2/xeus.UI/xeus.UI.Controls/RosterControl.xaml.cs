using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft.Samples.KMoore.WPFSamples.AnimatingTilePanel;
using xeus2.Properties;
using xeus2.xeus.Core;

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
        private DataTemplate _dataTemplateBig = null;
        private DataTemplate _dataTemplateMedium = null;
        private DataTemplate _dataTemplateSmall = null;

        private RosterItemSize _rosteritemSize = RosterItemSize.Small;

        public RosterControl()
        {
            Roster.Instance.Groups.LoadState();

            Settings.Default.PropertyChanged += Default_PropertyChanged;

            InitializeComponent();

            ItemSize = Settings.Default.UI_RosterItemSize;

            Roster.Instance.NeedRefresh += Instance_NeedRefresh;

            ListCollectionView listCollectionView =
                (ListCollectionView) CollectionViewSource.GetDefaultView(_roster.ItemsSource);
            listCollectionView.CustomSort = new RosterSort();
        }

        public ICollectionView CollectionView
        {
            get
            {
                return (ICollectionView) _roster.ItemsSource;
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

        private void Instance_NeedRefresh()
        {
            CollectionView.Refresh();
            Roster.Instance.Groups.Refresh();
        }

        public void SaveExpanderState()
        {
            Roster.Instance.Groups.SaveState();
        }

        private void Default_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "UI_RosterItemSize")
            {
                ItemSize = Settings.Default.UI_RosterItemSize;
            }
        }

        private void ChangeTemplate(DataTemplate dataTemplate)
        {
            _roster.BeginInit();
            _roster.ItemTemplate = dataTemplate;
            _roster.EndInit();
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
            Expander expander = (Expander) sender;
            string expanderName = ((CollectionViewGroup) expander.DataContext).Name.ToString();

            lock (Roster.Instance.Groups._syncObject)
            {
                Group group = Roster.Instance.Groups.FindGroup(expanderName);

                if (group == null)
                {
                    group = new Group(expanderName, true);
                }

                expander.DataContext = group;
            }

            bool expanded = Roster.Instance.IsGroupExpanded(expanderName);

            if (expanded != expander.IsExpanded)
            {
                expander.IsExpanded = Roster.Instance.IsGroupExpanded(expanderName);
            }
        }

        private void OnExpanded(object sender, RoutedEventArgs e)
        {
            Expander expander = (Expander) sender;
            Group group = (Group) expander.DataContext;

            group.IsExpanded = true;
        }

        private void OnCollapsed(object sender, RoutedEventArgs e)
        {
            Expander expander = (Expander) sender;
            Group group = (Group) expander.DataContext;

            group.IsExpanded = false;
        }
    }
}