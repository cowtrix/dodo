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
		[View(EPermissionLevel.PUBLIC, customDrawer:"null")]
		public string Parent { get; set; }

		public IRESTResource GetParent()
		{
			if(Guid.TryParse(Parent, out var guid))
			{
				return ResourceUtility.GetResourceByGuid(guid);
			}
			return ResourceUtility.GetResourceBySlug(Parent);
		}

		public OwnedResourceSchemaBase(string name, string publicDescription, string parent)
			: base(name, publicDescription)
		{
			Parent = parent;
		}

		public OwnedResourceSchemaBase() : base() { }
	}
}
