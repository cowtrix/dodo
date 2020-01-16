using REST.Security;
using Dodo.Rebellions;
using Dodo.Users;

namespace Dodo.Sites
{
	public class ActionSite : Site
	{
		public ActionSite()
		{
		}

		public ActionSite(User creator, Passphrase passphrase, Rebellion rebellion, SiteRESTHandler.CreationSchema schema) : base(creator, passphrase, rebellion, schema)
		{
		}
	}
}