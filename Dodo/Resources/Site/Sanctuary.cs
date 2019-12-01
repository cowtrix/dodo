using Common.Security;
using Dodo.Rebellions;
using Dodo.Users;

namespace Dodo.Sites
{
	public class Sanctuary : Site
	{
		public Sanctuary()
		{
		}

		public Sanctuary(User creator, Passphrase passphrase, Rebellion rebellion, SiteRESTHandler.CreationSchema schema) : base(creator, passphrase, rebellion, schema)
		{
		}
	}
}