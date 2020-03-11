using Common;
using Common.Extensions;
using Dodo.Resources;
using Resources;
using System;

namespace Dodo.Sites
{
	public sealed class SiteSchema : GroupResourceSchemaBase
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

	public class SiteFactory : DodoResourceFactory<Site, SiteSchema>, 
		IResourceFactory<EventSite>, IResourceFactory<MarchSite>, IResourceFactory<PermanentSite>
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

		EventSite IResourceFactory<EventSite>.CreateTypedObject(object context, ResourceSchemaBase schema)
		{
			return MakeTypedSite<EventSite>(context, schema);
		}

		PermanentSite IResourceFactory<PermanentSite>.CreateTypedObject(object context, ResourceSchemaBase schema)
		{
			return MakeTypedSite<PermanentSite>(context, schema);
		}

		MarchSite IResourceFactory<MarchSite>.CreateTypedObject(object context, ResourceSchemaBase schema)
		{
			return MakeTypedSite<MarchSite>(context, schema);
		}

		private T MakeTypedSite<T>(object context, ResourceSchemaBase schema) where T : Site
		{
			var site = CreateObject(context, schema);
			if(!(site is T typedSite))
			{
				throw new Exception($"Failed to create site {typeof(T)} with schema {schema.GetType()}");
			}
			return typedSite;
		}
	}
}
