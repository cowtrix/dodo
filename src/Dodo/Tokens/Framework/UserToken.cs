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
		Guid GUID { get; }
		void OnAdd(User parent);
	}

	[BsonDiscriminator(RootClass = true)]
	[BsonKnownTypes(
		typeof(ResetPasswordToken),
		typeof(AddAdminToken),
		typeof(TemporaryUserToken),
		typeof(VerifyEmailToken),
		typeof(SysAdminToken),
		typeof(ResourceCreationToken),
		typeof(SessionToken)
		)]
	public abstract class UserToken : IUserToken
	{
		public Guid GUID { get; private set; }

		public UserToken()
		{
			GUID = Guid.NewGuid();
		}

		public virtual void OnAdd(User parent)
		{
		}

		public bool CanVerify() => true;
	}
}