using System;

namespace Resources
{
	/// <summary>
	/// Fields and properties with this attribute will be serialized in REST api queries.
	/// </summary>
	public class ViewAttribute : Attribute {
		public EPermissionLevel ViewPermission { get; private set; }
		public EPermissionLevel EditPermission { get; private set; }

		public ViewAttribute(EPermissionLevel viewPermission, EPermissionLevel editPermission = EPermissionLevel.ADMIN)
		{
			if(viewPermission == EPermissionLevel.OWNER)
			{
				editPermission = EPermissionLevel.OWNER;
			}
			ViewPermission = viewPermission;
			EditPermission = editPermission;
		}
	}
}
