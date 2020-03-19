using Resources.Security;
using Dodo.Rebellions;
using Dodo.Users;
using Common;
using System;
using Resources;
using Newtonsoft.Json;

namespace Dodo.Sites
{
	public class MarchSite : EventSite, ITimeBoundResource
	{
		public MarchSite() : base(default, default) { }
		public MarchSite(AccessContext context, SiteSchema schema) : base(context, schema)
		{
		}
	}
}
