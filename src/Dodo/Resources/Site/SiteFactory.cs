using Dodo.DodoResources;
using Resources.Serializers;

namespace Dodo.LocationResources
{
	public class SiteSerializer : ResourceReferenceSerializer<Site> { }

	public class SiteFactory : DodoResourceFactory<Site, SiteSchema>
	{
	}
}
