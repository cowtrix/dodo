using Newtonsoft.Json;
using Resources;

namespace Dodo.Users.Tokens
{
	public class UserJoinedGroupToken : Token, IRemovableToken
	{
		[JsonProperty(TypeNameHandling = TypeNameHandling.None)]
		public ResourceReference<IGroupResource> Resource { get; private set; }

		public override bool Encrypted => true;

		public bool ShouldRemove => false;

		public UserJoinedGroupToken(IGroupResource rsc)
		{
			Resource = rsc.CreateRef();
		}

		public override EPermissionLevel GetVisibility() => EPermissionLevel.OWNER;

		public void OnRemove(AccessContext parent)
		{
		}
	}
}
