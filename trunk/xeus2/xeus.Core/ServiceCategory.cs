namespace xeus2.xeus.Core
{
	internal class ServiceCategory
	{
		private Services _services = new Services() ;
		private string _name ;

		public ServiceCategory( string name )
		{
			_name = name ;
		}

		public Services Items
		{
			get
			{
				return _services ;
			}
		}


		public Services Services
		{
			get
			{
				return _services ;
			}
		}

		public string Name
		{
			get
			{
				return _name ;
			}
		}

		public override string ToString()
		{
			return Name ;
		}
	}
}