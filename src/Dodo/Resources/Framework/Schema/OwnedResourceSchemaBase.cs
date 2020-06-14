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
		public Guid Parent { get; set; }

		[View]
		[JsonIgnore]
		[BsonIgnore]
		[Name("Parent Resource")]
		public ResourceReference<IRESTResource> ParentReference => new ResourceReference<IRESTResource>(Parent);

		public OwnedResourceSchemaBase(string name, string publicDescription, Guid parent)
			: base(name, publicDescription)
		{
			Parent = parent;
		}

		public OwnedResourceSchemaBase() : base() { }
	}
}
