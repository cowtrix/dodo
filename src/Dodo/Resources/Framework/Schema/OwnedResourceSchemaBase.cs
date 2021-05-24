using Common;
using Resources;
using System;

namespace Dodo
{
	public abstract class OwnedResourceSchemaBase : DescribedResourceSchemaBase
	{
		[Name("Parent")]
		[View(EPermissionLevel.PUBLIC, customDrawer:"parentRefString", priority:-1)]
		public string ParentID { get; set; }

		public IRESTResource GetParent()	
		{
			if(Guid.TryParse(ParentID, out var guid))
			{
				return ResourceUtility.GetResourceByGuid(guid);
			}
			return ResourceUtility.GetResourceBySlug(ParentID);
		}

		public OwnedResourceSchemaBase(string name, string publicDescription, string parent)
			: base(name, publicDescription)
		{
			ParentID = parent;
		}

		public OwnedResourceSchemaBase() : base() { }
	}
}
