using Common;

namespace Dodo.Sites
{
	[Name("Site")]
	public class PermanentSite : Site
	{
		public PermanentSite() : base() { }
		public PermanentSite(AccessContext context, SiteSchema schema) : base(context, schema)
		{
		}
	}
}
