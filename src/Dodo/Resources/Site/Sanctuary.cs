using Resources.Security;
using Dodo.Rebellions;
using Dodo.Users;
using Common;

namespace Dodo.Sites
{
	public class Sanctuary : Site
	{
		public Sanctuary() : base(default, default) { }
		public Sanctuary(AccessContext context, SiteSchema schema) : base(context, schema)
		{
		}
	}
}