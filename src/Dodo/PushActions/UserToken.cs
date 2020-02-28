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
		typeof(AdminToken)
		)]
	public abstract class UserToken
	{
		public Guid GUID { get; private set; }
		public virtual bool AutoFire { get { return false; } }
		[JsonProperty]
		public bool HasExecuted { get; private set; }
		[JsonProperty]
		public virtual bool CanRemove { get { return false; } }

		public UserToken()
		{
			GUID = Guid.NewGuid();
		}

		public void Execute(AccessContext context)
		{
			if(HasExecuted)
			{
				return;
			}
			ExecuteInternal(context);
			HasExecuted = true;
		}

		protected virtual void ExecuteInternal(AccessContext context) { }

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
