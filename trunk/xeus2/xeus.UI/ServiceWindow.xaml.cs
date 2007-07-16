using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using agsXMPP.protocol.x.data;
using xeus2.xeus.Commands;
using xeus2.xeus.Core;

namespace xeus2.xeus.UI
{
    public class PropertyObjectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Service service = (Service)value;

            if (service.XDataCollection != null)
            {
                foreach (Field field in service.XDataCollection)
                {
                    if (field.Var == (string) parameter)
                    {
                        return field.GetValues()[0];
                    }
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Interaction logic for ServiceWindow.xaml
    /// </summary>
    public partial class ServiceWindow : Window
    {
        public ServiceWindow()
        {
            InitializeComponent();

            Services.Instance.MucRooms.CollectionChanged +=
                new NotifyCollectionChangedEventHandler(MucRooms_CollectionChanged);
        }

        private void MucRooms_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        GridView view = (GridView)_mucResult.View;

                        foreach (Service service in e.NewItems)
                        {
                            if (service.XDataCollection != null)
                            {
                                AddColumns(service, view);
                            }
                            else
                            {
                                service.PropertyChanged +=
                                    new System.ComponentModel.PropertyChangedEventHandler(service_PropertyChanged);
                            }
                        }

                        break;
                    }
            }
        }

        void service_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            GridView view = (GridView) _mucResult.View;

            if (e.PropertyName == "XDataCollection")
            {
                AddColumns((Service)sender, view);
            }
        }

        private void AddColumns(Service service, GridView view)
        {
            foreach (Field field in service.XDataCollection)
            {
                bool exists = false;

                foreach (GridViewColumn existingColumn in view.Columns)
                {
                    if ( existingColumn.Header.ToString() == field.Label )
                    {
                        exists = true;
                        break;
                    }
                }

                if (exists)
                {
                    continue;
                }

                Binding binding = new Binding();
                binding.Converter = new PropertyObjectConverter();
                binding.ConverterParameter = field.Var;
                binding.Mode = BindingMode.OneWay;
                /*
                FrameworkElementFactory textElement = new FrameworkElementFactory(typeof(TextBlock));
                textElement.SetBinding(TextBlock.TextProperty, binding);

                DataTemplate template = new DataTemplate();
                template.VisualTree = textElement;
                */

                GridViewColumn column = new GridViewColumn();
                //column.CellTemplate = template;
                column.Header = field.Label;
                column.DisplayMemberBinding = binding;

                view.Columns.Add(column);
            }
        }

        public override void EndInit()
        {
            base.EndInit();

            new TextFilterService(_servicesResult.ItemsSource as ListCollectionView, _filter);
            new TextFilterService(_mucResult.ItemsSource as ListCollectionView, _filterMuc);

            ServiceCommands.RegisterCommands(this);
        }
    }
}