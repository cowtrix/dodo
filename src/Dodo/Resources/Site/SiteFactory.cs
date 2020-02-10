using Common;
using Common.Extensions;
using Dodo.Resources;
using REST;
using System;

namespace Dodo.Sites
{
	public class SiteSchema : DodoResourceSchemaBase
	{
		public string Type { get; private set; }
		public Guid Parent { get; private set; }
		public GeoLocation Location { get; private set; }
		public string PublicDescription { get; private set; }

		public SiteSchema(AccessContext context, string name, string type, GroupResource parent, GeoLocation location, string publicDescription)
			: base(context, name)
		{
			Type = type;
			Parent = parent.GUID;
			Location = location;
			PublicDescription = publicDescription;
		}

		public SiteSchema(string name, string type, Guid parent, GeoLocation location, string description)
			: base(name)
		{
			Type = type;
			Location = location;
			Parent = parent;
			PublicDescription = description;
		}

		public SiteSchema()
		{
		}
	}

	public class SiteFactory : DodoResourceFactory<Site, SiteSchema> 
	{
		protected override Site CreateObjectInternal(SiteSchema schema)
		{
			var siteType = Type.GetType(schema.Type);
			if(siteType == null)
			{
				throw new Exception("Invalid Site Type: " + schema.Type);
			}
			var newSite = Activator.CreateInstance(siteType, schema) as Site;
			if(!newSite.Verify(out var error))
			{
				throw new Exception(error);
			}
			return newSite;
		}
	}
}