using REST.Security;
using Dodo.Rebellions;
using Dodo.Users;

namespace Dodo.Sites
{
	public class OccupationalSite : Site
	{
		public OccupationalSite()
		{
		}

		public OccupationalSite(User creator, Passphrase passphrase, Rebellion rebellion, SiteRESTHandler.CreationSchema schema) : base(creator, passphrase, rebellion, schema)
		{
		}
	}
}