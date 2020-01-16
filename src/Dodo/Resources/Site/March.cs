using REST.Security;
using Dodo.Rebellions;
using Dodo.Users;

namespace Dodo.Sites
{
	public class March : Site
	{
		public March()
		{
		}

		public March(User creator, Passphrase passphrase, Rebellion rebellion, SiteRESTHandler.CreationSchema schema) : base(creator, passphrase, rebellion, schema)
		{
		}
	}
}