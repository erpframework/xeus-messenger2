using agsXMPP.protocol.x.muc ;
using xeus2.Properties ;

namespace xeus2.xeus.Core
{
	internal static class MucStatusCodeTexts
	{
		public static string GetCodeText( Status status )
		{
			switch ( status.Code )
			{
				case StatusCode.FullJidVisible:
					{
						return Resources.MucStatus_100 ;
					}
				case StatusCode.AffiliationChanged:
					{
						return Resources.MucStatus_101 ;
					}
				case StatusCode.ShowUnavailableMembers:
					{
						return Resources.MucStatus_102 ;
					}
				case StatusCode.HideUnavailableMembers:
					{
						return Resources.MucStatus_103 ;
					}
				case StatusCode.ConfigurationChanged:
					{
						return Resources.MucStatus_104 ;
					}
				case StatusCode.RoomCreated:
					{
						return Resources.MucStatus_201 ;
					}
				case StatusCode.Banned:
					{
						return Resources.MucStatus_301 ;
					}
				case StatusCode.NewNickname:
					{
						return Resources.MucStatus_303 ;
					}
				case StatusCode.Kicked:
					{
						return Resources.MucStatus_307 ;
					}
				case StatusCode.AffiliationChange:
					{
						return Resources.MucStatus_321 ;
					}
				case StatusCode.MembersOnly:
					{
						return Resources.MucStatus_322 ;
					}
				case StatusCode.Shutdown:
					{
						return Resources.MucStatus_332 ;
					}
				case StatusCode.Unknown:
					{
						switch ( status.GetAttribute( "code" ) )
						{
							case "110":
								{
									return Resources.MucStatus_110 ;
								}
							case "170":
								{
									return Resources.MucStatus_170 ;
								}
							case "171":
								{
									return Resources.MucStatus_171 ;
								}
							case "172":
								{
									return Resources.MucStatus_172 ;
								}
							case "173":
								{
									return Resources.MucStatus_173 ;
								}
							case "174":
								{
									return Resources.MucStatus_174 ;
								}
							case "210":
								{
									return Resources.MucStatus_210 ;
								}
						}

						break ;
					}
				default:
					{
						return Resources.MucStatus_000 ;
					}
			}

			return null ;
		}
	}
}