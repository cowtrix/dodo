using Common;
using REST.Security;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Dodo.Users
{
	public class SingletonPushActionAttribute : Attribute
	{
	}

	public class PushActionDuplicateException : Exception
	{
		public PushActionDuplicateException(string message) : base(message)
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
	public abstract class PushAction
	{
		public Guid GUID { get; private set; }
		public virtual bool AutoFire { get { return false; } }
		[JsonProperty]
		public bool HasExecuted { get; private set; }
		[JsonProperty]
		public virtual bool CanRemove { get { return false; } }

		public PushAction()
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
