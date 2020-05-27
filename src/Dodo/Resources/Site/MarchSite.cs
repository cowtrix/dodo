using Common;

namespace Dodo.Sites
{
	[Name("March")]
	public class MarchSite : EventSite, ITimeBoundResource
	{
		public MarchSite() : base() { }
		public MarchSite(AccessContext context, SiteSchema schema) : base(context, schema)
		{
		}
	}
}
