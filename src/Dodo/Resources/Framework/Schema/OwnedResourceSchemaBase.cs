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

		public OwnedResourceSchemaBase(string name, string publicDescription, Guid parent)
			: base(name, publicDescription)
		{
			Parent = parent;
		}

		public OwnedResourceSchemaBase() : base() { }
	}
}
