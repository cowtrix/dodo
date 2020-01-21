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
		public GroupResource Parent { get; private set; }
		public GeoLocation Location { get; private set; }
		public string PublicDescription { get; private set; }

		public SiteSchema(AccessContext context, string name, string type, GroupResource parent, GeoLocation location, string publicDescription)
			: base(context, name)
		{
			Type = type;
			Parent = parent;
			Location = location;
			PublicDescription = publicDescription;
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
				throw new HttpException("Invalid Site Type", System.Net.HttpStatusCode.BadRequest);
			}
			var newSite = Activator.CreateInstance(siteType, schema) as Site;
			newSite.Verify();
			return newSite;
		}
	}
}