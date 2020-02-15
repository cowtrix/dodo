using Resources.Security;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Resources;
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
		[View(EPermissionLevel.PUBLIC)]
		public IEnumerable<PushAction> Actions { get { return m_actions; } }

		[JsonProperty]
		[BsonElement]
		private List<PushAction> m_actions = new List<PushAction>();

		public void Add(PushAction pa)
		{
			var type = pa.GetType();
			var isSingleton = type.GetCustomAttribute<SingletonPushActionAttribute>();
			if (isSingleton != null && Actions.Any(action => action.GetType() == type))
			{
				throw new PushActionDuplicateException($"Cannot have multiple {type} PushActions");
			}
			m_actions.Add(pa);
			pa.OnAdd();
		}

		public void Remove(PushAction pa)
		{
			if(!pa.CanRemove)
			{
				throw new System.Exception("This PushAction cannot be dismissed");
			}
			pa.OnRemove();
			m_actions.Remove(pa);
		}

		public T GetSinglePushAction<T>() where T:PushAction
		{
			return Actions.SingleOrDefault(pa => pa is T) as T;
		}
	}

}
