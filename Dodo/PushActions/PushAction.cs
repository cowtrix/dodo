using Common;
using Common.Security;
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

	public abstract class PushAction
	{
		public abstract string Message { get; }
		public abstract bool AutoFire { get; }
		[JsonProperty]
		public bool HasExecuted { get; private set; }

		protected virtual void ExecuteInternal(User user, Passphrase passphrase) { }
		public void Execute(User user, Passphrase passphrase)
		{
			if(HasExecuted)
			{
				return;
			}
			ExecuteInternal(user, passphrase);
			HasExecuted = true;
		}
	}

}
