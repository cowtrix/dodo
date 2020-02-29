using Common;
using Resources.Security;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Dodo.Users
{
	public class SingletonTokenAttribute : Attribute
	{
	}

	public class SingletonTokenDuplicateException : Exception
	{
		public SingletonTokenDuplicateException(string message) : base(message)
		{
		}
	}

	[BsonDiscriminator(RootClass = true)]
	[BsonKnownTypes(
		typeof(ResetPasswordAction),
		typeof(AddAdminAction),
		typeof(TemporaryUserAction),
		typeof(VerifyEmailAction),
		typeof(SysAdminToken)
		)]
	public abstract class UserToken
	{
		public Guid GUID { get; private set; }
		[JsonProperty]
		public virtual bool CanRemove { get { return false; } }

		[JsonProperty]
		protected virtual bool Removed { get; set; }

		public UserToken()
		{
			GUID = Guid.NewGuid();
		}

		public virtual void OnAdd()
		{
		}

		public virtual void OnRemove()
		{
		}

		public virtual string GetNotificationMessage()
		{
			return null;
		}
	}
}
