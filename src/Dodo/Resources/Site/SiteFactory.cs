using Common;
using Common.Extensions;
using Dodo.Resources;
using Resources;
using System;

namespace Dodo.Sites
{
	public class SiteSchema : GroupResourceSchemaBase
	{
		public string Type { get; private set; }
		public GeoLocation Location { get; private set; }

		public SiteSchema(string name, string type, Guid parent, GeoLocation location, string description)
			: base(name, description, parent)
		{
			Type = type;
			Location = location;
		}

		public SiteSchema()
		{
		}
	}

	public abstract class TimeboundSiteSchema : SiteSchema
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

	public class SiteFactory : DodoResourceFactory<Site, SiteSchema>, 
		IResourceFactory<ActionSite>, IResourceFactory<EventSite>, IResourceFactory<MarchSite>, IResourceFactory<OccupationSite>, IResourceFactory<SanctuarySite>
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

		ActionSite IResourceFactory<ActionSite>.CreateTypedObject(object context, ResourceSchemaBase schema)
		{
			return CreateObject(context, schema) as ActionSite;
		}

		EventSite IResourceFactory<EventSite>.CreateTypedObject(object context, ResourceSchemaBase schema)
		{
			return CreateObject(context, schema) as EventSite;
		}

		OccupationSite IResourceFactory<OccupationSite>.CreateTypedObject(object context, ResourceSchemaBase schema)
		{
			return CreateObject(context, schema) as OccupationSite;
		}

		SanctuarySite IResourceFactory<SanctuarySite>.CreateTypedObject(object context, ResourceSchemaBase schema)
		{
			return CreateObject(context, schema) as SanctuarySite;
		}

		MarchSite IResourceFactory<MarchSite>.CreateTypedObject(object context, ResourceSchemaBase schema)
		{
			return CreateObject(context, schema) as MarchSite;
		}
	}
}