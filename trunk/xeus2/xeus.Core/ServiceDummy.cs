using System;
using System.Collections.Generic;
using System.Text;
using agsXMPP.protocol.iq.disco ;

namespace xeus2.xeus.Core
{
	internal class ServiceDummy : Service
	{
		private readonly Services _parentCollection ;
		private readonly DiscoItem _parent ;

		public ServiceDummy( Services parentCollection, DiscoItem parent  ) : base( null, false )
		{
			_parentCollection = parentCollection ;
			_parent = parent ;
		}

		public new string Name
		{
			get
			{
				_parentCollection.Remove( this ) ;

				Account.Instance.Discovery( _parent ) ;

				return "dummy" ;
			}
		}
	}
}
