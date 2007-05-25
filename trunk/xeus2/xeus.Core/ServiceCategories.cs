namespace xeus2.xeus.Core
{
	internal class ServiceCategories : ObservableCollectionDisp<ServiceCategory>
	{
		public void AddService( Service service )
		{
			lock ( _syncObject )
			{
				foreach ( string categoryName in service.Categories )
				{
					bool exists = false ;
					foreach ( ServiceCategory category in Items )
					{
						if ( category.Name == categoryName )
						{
							category.Services.Add( service );
							exists = true ;
							break ;
						}
					}

					if ( !exists )
					{
						ServiceCategory serviceCategory = new ServiceCategory( categoryName ) ;
						Add( serviceCategory );
						serviceCategory.Services.Add( service );
					}
				}
			}
		}
	}
}