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
	/// This collection enforces some restrictions on tokens, e.g. there can only be
	/// one instance of some tokens in a list
	/// </summary>
	public class TokenCollection
	{
		public IEnumerable<UserToken> Actions { get { return m_actions; } }

		[JsonProperty]
		[BsonElement]
		private List<UserToken> m_actions = new List<UserToken>();

		public void Add(UserToken pa)
		{
			var type = pa.GetType();
			var isSingleton = type.GetCustomAttribute<SingletonTokenAttribute>();
			if (isSingleton != null && Actions.Any(action => action.GetType() == type))
			{
				throw new SingletonTokenDuplicateException($"Cannot have multiple {type} Tokens");
			}
			m_actions.Add(pa);
			pa.OnAdd();
		}

		public void Remove(UserToken pa)
		{
			if(!pa.CanRemove)
			{
				throw new System.Exception("This Token cannot be dismissed");
			}
			pa.OnRemove();
			m_actions.Remove(pa);
		}

		public T GetSingleToken<T>() where T:UserToken
		{
			return Actions.SingleOrDefault(pa => pa is T) as T;
		}
	}

}
