using Newtonsoft.Json;
using SimpleHttpServer.REST;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dodo.Users
{
	/// <summary>
	/// This collection enforces some restrictions on push actions, e.g. there can only be
	/// one instance of some actions in a list
	/// </summary>
	public class PushActionCollection
	{
		[JsonProperty]
		[View(EPermissionLevel.PUBLIC)]
		public List<PushAction> Actions { get; private set; }

		public PushActionCollection()
		{
			Actions = new List<PushAction>();
		}

		public void Add(PushAction pa)
		{
			var type = pa.GetType();
			var isSingleton = type.GetCustomAttribute<SingletonPushActionAttribute>();
			if (isSingleton != null && Actions.Any(action => action.GetType() == type))
			{
				throw new PushActionDuplicateException($"Cannot have multiple {type} PushActions");
			}
			Actions.Add(pa);
		}

		public T GetSinglePushAction<T>() where T:PushAction
		{
			return Actions.SingleOrDefault(pa => pa is T) as T;
		}
	}

}
