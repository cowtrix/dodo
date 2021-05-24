using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using Common.Extensions;
using Resources;

namespace Dodo.Users.Tokens
{
	public interface IToken : IVerifiable
	{
		Guid Guid { get; }
		bool Encrypted { get; }
		void OnAdd(ITokenResource parent);
		EPermissionLevel GetVisibility();
	}

	[BsonDiscriminator(RootClass = true)]
	[BsonKnownTypes(
		typeof(ResetPasswordToken),
		typeof(UserAddedAsAdminToken),
		typeof(TemporaryUserToken),
		typeof(VerifyEmailToken),
		typeof(ResourceCreationToken),
		typeof(SessionToken),
		typeof(EncryptedNotificationToken),
		typeof(SimpleNotificationToken)
		)]
	public abstract class Token : IToken
	{
		[JsonProperty]
		[BsonElement]
		public Guid Guid { get; private set; } = Guid.NewGuid();

		[JsonIgnore]
		[BsonIgnore]
		public abstract bool Encrypted { get; }

		public virtual void OnAdd(ITokenResource parent) { }

		public bool CanVerify() => true;

		public virtual bool VerifyExplicit(out string error)
		{
			error = null;
			return true;
		}

		public abstract EPermissionLevel GetVisibility();
	}
}
