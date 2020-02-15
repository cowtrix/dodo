using Common;
using Common.Extensions;
using Dodo.Resources;
using Resources;
using System;

namespace Dodo.Sites
{
	public class SiteSchema : DodoResourceSchemaBase
	{
		public string Type { get; private set; }
		public Guid Parent { get; private set; }
		public GeoLocation Location { get; private set; }
		public string PublicDescription { get; private set; }

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

	public class TimeboundSiteSchema : SiteSchema
	{
		public DateTime StartDate { get; private set; }
		public DateTime EndDate { get; private set; }

		public TimeboundSiteSchema(string name, string type, Guid parent, GeoLocation location, string description, DateTime start, DateTime end) : 
			base(name, type, parent, location, description)
		{
			StartDate = start;
			EndDate = end;
		}

		public TimeboundSiteSchema()
		{
		}
	}

	public class SiteFactory : DodoResourceFactory<Site, SiteSchema> 
	{
		protected override Site CreateObjectInternal(AccessContext context, SiteSchema schema)
		{
			var siteType = Type.GetType(schema.Type);
			if(siteType == null)
			{
				throw new Exception("Invalid Site Type: " + schema.Type);
			}
			var newSite = Activator.CreateInstance(siteType, context, schema) as Site;
			if(!newSite.Verify(out var error))
			{
				throw new Exception(error);
			}
			return newSite;
		}
	}
}