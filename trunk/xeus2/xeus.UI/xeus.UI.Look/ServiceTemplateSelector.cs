using System.Windows ;
using System.Windows.Controls ;
using xeus2.xeus.Core ;

namespace xeus2.xeus.UI.Look
{
	internal class ServiceTemplateSelector : DataTemplateSelector
	{
		private DataTemplate _serviceTemplate ;

		public DataTemplate ServiceTemplate
		{
			get
			{
				return _serviceTemplate ;
			}
			set
			{
				_serviceTemplate = value ;
			}
		}

		private DataTemplate _serviceCategoryTemplate ;

		public DataTemplate ServiceCategoryTemplate
		{
			get
			{
				return _serviceCategoryTemplate ;
			}
			set
			{
				_serviceCategoryTemplate = value ;
			}
		}

		public override DataTemplate SelectTemplate( object item, DependencyObject container )
		{
			if ( item is ServiceCategory )
			{
				return _serviceCategoryTemplate ;
			}
			else
			{
				return _serviceTemplate ;
			}
		}
	}
}