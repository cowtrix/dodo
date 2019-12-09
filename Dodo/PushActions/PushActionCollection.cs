using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dodo.Users
{
	public class PushActionCollection
	{
		[JsonProperty]
		private List<PushAction> m_actions = new List<PushAction>();

		public IEnumerable<PushAction> AllActions { get { return m_actions; } }

		public void Add(PushAction pa)
		{
			var type = pa.GetType();
			var isSingleton = type.GetCustomAttribute<SingletonPushActionAttribute>();
			if (isSingleton != null && m_actions.Any(action => action.GetType() == type))
			{
				throw new PushActionDuplicateException($"Cannot have multiple {type} PushActions");
			}
			m_actions.Add(pa);
		}

		public T GetSinglePushAction<T>() where T:PushAction
		{
			return m_actions.SingleOrDefault(pa => pa is T) as T;
		}
	}

}
