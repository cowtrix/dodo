using Newtonsoft.Json;
using Resources;

namespace Dodo.Users.Tokens
{
	public interface IMyRebellionToken : IToken
	{
		public IResourceReference Reference { get; }
	}
	public class UserJoinedGroupToken : Token, IRemovableToken, IMyRebellionToken
	{
		[JsonProperty(TypeNameHandling = TypeNameHandling.None)]
		public ResourceReference<IGroupResource> Resource { get; private set; }

		public override bool Encrypted => true;

		public bool ShouldRemove => false;

		[JsonIgnore]
		public IResourceReference Reference => Resource;

		public UserJoinedGroupToken(IGroupResource rsc)
		{
			Resource = rsc.CreateRef();
		}

		public override EPermissionLevel GetVisibility() => EPermissionLevel.ADMIN;

		public void OnRemove(AccessContext parent)
		{
		}
	}
}
