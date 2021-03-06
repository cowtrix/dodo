using System;

namespace Resources
{
	public class ViewDrawerAttribute : Attribute
	{
		public string DrawerName { get; private set; }
		public ViewDrawerAttribute(string drawerName)
		{
			DrawerName = drawerName;
		}
	}

	/// <summary>
	/// Fields and properties with this attribute will be serialized in REST api queries.
	/// </summary>
	public class ViewAttribute : Attribute {
		public EPermissionLevel ViewPermission { get; private set; }
		public EPermissionLevel EditPermission { get; private set; }
		public int Priority { get; private set; }
		public string CustomDrawer { get; private set; }
		public string InputHint { get; set; }

		public ViewAttribute(
			EPermissionLevel viewPermission, 
			EPermissionLevel editPermission = EPermissionLevel.ADMIN, 
			int priority = 255, 
			string customDrawer = null,
			string inputHint = null)
		{
			if(viewPermission == EPermissionLevel.ADMIN)
			{
				editPermission = EPermissionLevel.ADMIN;
			}
			ViewPermission = viewPermission;
			EditPermission = editPermission;
			Priority = priority;
			CustomDrawer = customDrawer;
			InputHint = inputHint;
		}

		public ViewAttribute() : this(EPermissionLevel.PUBLIC, EPermissionLevel.PUBLIC)
		{
		}
	}
}
