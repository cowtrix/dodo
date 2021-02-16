using Common;
using Resources.Security;
using Resources;
using Resources.Location;
using Dodo.Users.Tokens;
using System.Threading.Tasks;

namespace Dodo.LocationResources
{
	public enum EArrestRisk
	{
		High,
		Moderate,
		Low,
		None,
	}

	public enum EAccessType
	{
		None,
		Free,
		Paid,
	}

	public abstract class LocationResourceBase : GroupResource,
		ILocationalResource, IOwnedResource, IVideoResource, IPublicResource
	{
		[View(EPermissionLevel.PUBLIC)]
		[Name("Arrest Risk")]
		public EArrestRisk ArrestRisk { get; set; }
		[View(EPermissionLevel.PUBLIC, priority: 512)]
		public SiteFacilities Facilities { get; set; }		
		[View(EPermissionLevel.PUBLIC)]
		[Name("Video Embed URL")]
		public string VideoEmbedURL { get; set; }
		[View(EPermissionLevel.PUBLIC)]
		public GeoLocation Location { get; set; } = new GeoLocation();
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM, priority: -2, customDrawer: "parentRef")]
		public ResourceReference<IRESTResource> Parent { get; set; }
		

		public LocationResourceBase() : base() { }

		public LocationResourceBase(AccessContext context, LocationResourceSchema schema) : base(context, schema)
		{
			if (schema == null)
			{
				return;
			}
			var group = schema.GetParent();
			Parent = group.CreateRef();
			Location = schema.Location;
			// force location lookup
			new Task(() => LocationManager.GetLocationData(Location)).Start();
			PublicDescription = schema.PublicDescription;
			Facilities = schema.Facilities;
			VideoEmbedURL = schema.VideoEmbedURL;
		}

		public override Passphrase GetPrivateKey(AccessContext context)
		{
			return Parent.GetValue<ITokenResource>().GetPrivateKey(context);
		}
	}
}
