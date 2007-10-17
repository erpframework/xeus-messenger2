using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace xeus2.xeus.Utilities
{
    /// <summary>
    /// This class provides utilities for use with WPF markup. The majority of
    /// the utilities are in the form of Attached <see cref="DependencyProperty">
    /// Dependency Properties</see>
    /// </summary>
    public class WPFUtils
    {
        #region Dependency Properties
        /// <summary>
        /// Set this Attached DependencyProperty on a ListView or a PowerGrid
        /// to enable sorting on its columns.
        /// </summary>
        public static readonly DependencyProperty IsGridSortableProperty;
        private static readonly DependencyPropertyKey LastSortedPropertyKey;
        private static readonly DependencyPropertyKey LastSortDirectionPropertyKey;

        #endregion

        #region Static Constructor

        static WPFUtils()
        {
            IsGridSortableProperty = DependencyProperty.RegisterAttached(
                "IsGridSortable",
                typeof(Boolean),
                typeof(WPFUtils),
                new PropertyMetadata(new PropertyChangedCallback(OnRegisterSortableGrid)));
            LastSortDirectionPropertyKey = DependencyProperty.RegisterAttachedReadOnly(
                "LastSortDirection",
                typeof(ListSortDirection),
                typeof(WPFUtils),
                new PropertyMetadata());
            LastSortedPropertyKey = DependencyProperty.RegisterAttachedReadOnly(
                "LastSorted",
                typeof(GridViewColumnHeader),
                typeof(WPFUtils),
                new PropertyMetadata());
        }

        #endregion

        #region Attached Property Setters/Getters

        public static Boolean GetIsGridSortable(DependencyObject obj)
        {
            return (Boolean)obj.GetValue(IsGridSortableProperty);
        }

        public static void SetIsGridSortable(DependencyObject obj, Boolean value)
        {
            obj.SetValue(IsGridSortableProperty, value);
        }

        public static GridViewColumnHeader GetLastSorted(DependencyObject obj)
        {
            return (GridViewColumnHeader)obj.GetValue(LastSortedPropertyKey.DependencyProperty);
        }

        private static void SetLastSorted(DependencyObject obj, GridViewColumnHeader value)
        {
            obj.SetValue(LastSortedPropertyKey, value);
        }

        public static ListSortDirection GetLastSortDirection(DependencyObject obj)
        {
            return (ListSortDirection)obj.GetValue(LastSortDirectionPropertyKey.DependencyProperty);
        }

        private static void SetLastSortDirection(DependencyObject obj, ListSortDirection value)
        {
            obj.SetValue(LastSortDirectionPropertyKey, value);
        }

        #endregion

        #region PropertyChangedHandlers

        private static void OnRegisterSortableGrid(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ListView grid = sender as ListView;
            if (grid != null)
            {
                RegisterSortableGridview(grid, args);
            }
        }

        #endregion //PropertyChangedHandlers

        private static void RegisterSortableGridview(ListView grid, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue is Boolean && (Boolean)args.NewValue)
            {
                grid.AddHandler(GridViewColumnHeader.ClickEvent, GridViewColumnHeaderClickHandler);
            }
            else
            {
                grid.RemoveHandler(GridViewColumnHeader.ClickEvent, GridViewColumnHeaderClickHandler);
            }
        }

        private static RoutedEventHandler GridViewColumnHeaderClickHandler = new RoutedEventHandler(GridViewColumnHeaderClicked);

        private static void GridViewColumnHeaderClicked(object sender, RoutedEventArgs e)
        {
            ListView lv = sender as ListView;
            if (lv != null)
            {
                GridViewColumnHeader header = e.OriginalSource as GridViewColumnHeader;
                if (header != null)
                {
                    ListSortDirection sortDirection;
                    GridViewColumnHeader tmpHeader = GetLastSorted(lv);
                    if (tmpHeader != null)
                        tmpHeader.Column.HeaderTemplate = null;
                    if (header != tmpHeader)
                    {
                        sortDirection = ListSortDirection.Ascending;
                    }
                    else
                    {
                        ListSortDirection tmpDirection = GetLastSortDirection(lv);
                        if (tmpDirection == ListSortDirection.Ascending)
                            sortDirection = ListSortDirection.Descending;
                        else
                            sortDirection = ListSortDirection.Ascending;
                    }
                    SetLastSorted(lv, header);
                    SetLastSortDirection(lv, sortDirection);
                    string resourceTemplateName = "";
                    switch (sortDirection)
                    {
                        case ListSortDirection.Ascending: resourceTemplateName = "HeaderTemplateSortAsc"; break;
                        case ListSortDirection.Descending: resourceTemplateName = "HeaderTemplateSortDesc"; break;
                    }
                    DataTemplate tmpTemplate = lv.TryFindResource(resourceTemplateName) as DataTemplate;
                    if (tmpTemplate != null)
                    {
                        header.Column.HeaderTemplate = tmpTemplate;
                    }
                    Sort(lv);
                }
            }
        }

        private static void Sort(ListView lv)
        {
            Cursor oldCursor = lv.Cursor;
            lv.Cursor = Cursors.Wait;

            Binding binding = (Binding) GetLastSorted(lv).Column.DisplayMemberBinding;

            if (binding != null)
            {
                string headerProperty = ((Binding) GetLastSorted(lv).Column.DisplayMemberBinding).Path.Path;

                ICollectionView dataView =
                    CollectionViewSource.GetDefaultView(lv.ItemsSource);

                dataView.SortDescriptions.Clear();
                dataView.SortDescriptions.Add(new SortDescription(headerProperty, GetLastSortDirection(lv)));
                dataView.Refresh();
            }

            lv.Cursor = oldCursor;
        }

        static void RefreshCommandsInternal()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public static void RefreshCommands()
        {
            App.InvokeSafe(App._dispatcherPriority, new RefreshCommandHandler(RefreshCommandsInternal));
        }

        delegate void RefreshCommandHandler();
    }
}
