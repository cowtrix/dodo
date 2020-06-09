using Common;
using Resources.Security;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Linq;
using Common.Extensions;

namespace Dodo.Users.Tokens
{
	public interface IToken : IVerifiable
	{
		Guid Guid { get; }
		bool Encrypted { get; }
		void OnAdd(ITokenOwner parent);
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
		public Guid Guid { get; private set; }
		[JsonIgnore]
		[BsonIgnore]
		public abstract bool Encrypted { get; }

		public virtual void OnAdd(ITokenOwner parent)
		{
			Guid = Guid.NewGuid();
		}

		public bool CanVerify() => true;

		public virtual bool VerifyExplicit(out string error)
		{
			error = null;
			return true;
		}
	}
}
