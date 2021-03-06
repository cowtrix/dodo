using Common;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Resources;
using System;
using System.ComponentModel;

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
