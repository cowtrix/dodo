using Common;
using Resources.Security;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Linq;
using Common.Extensions;

namespace Dodo.Users.Tokens
{
	public interface IUserToken : IVerifiable
	{
		Guid Guid { get; }
		bool Encrypted { get; }
		void OnAdd(User parent);
	}

	[BsonDiscriminator(RootClass = true)]
	[BsonKnownTypes(
		typeof(ResetPasswordToken),
		typeof(AddAdminToken),
		typeof(TemporaryUserToken),
		typeof(VerifyEmailToken),
		typeof(ResourceCreationToken),
		typeof(SessionToken)
		)]
	public abstract class UserToken : IUserToken
	{
		[JsonProperty]
		[BsonElement]
		public Guid Guid { get; private set; }
		[JsonIgnore]
		[BsonIgnore]
		public abstract bool Encrypted { get; }

		public virtual void OnAdd(User parent)
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
