using Common.Security;
using Dodo.Rebellions;
using Dodo.Users;

namespace Dodo.Sites
{
	public class Event : Site
	{
		public Event()
		{
		}

		public Event(User creator, Passphrase passphrase, Rebellion rebellion, SiteRESTHandler.CreationSchema schema) : base(creator, passphrase, rebellion, schema)
		{
		}
	}
}