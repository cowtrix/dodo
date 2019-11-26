using System;

namespace SimpleHttpServer.REST
{
	/// <summary>
	/// Fields and properties with this attribute will be serialized in REST api queries.
	/// </summary>
	public class ViewAttribute : Attribute {
		public EUserPriviligeLevel ViewPermission { get; private set; }
		public EUserPriviligeLevel EditPermission { get; private set; }

		public ViewAttribute(EUserPriviligeLevel viewPermission, EUserPriviligeLevel editPermission = EUserPriviligeLevel.ADMIN)
		{
			if(viewPermission == EUserPriviligeLevel.OWNER)
			{
				editPermission = EUserPriviligeLevel.OWNER;
			}
			ViewPermission = viewPermission;
			EditPermission = editPermission;
		}
	}
}
