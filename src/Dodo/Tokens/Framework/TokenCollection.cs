using Resources.Security;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Resources;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using Common.Extensions;

namespace Dodo.Users.Tokens
{
	/// <summary>
	/// This collection enforces some restrictions on tokens, e.g. there can only be
	/// one instance of some tokens in a list
	/// </summary>
	public class TokenCollection
	{
		public IEnumerable<UserToken> Tokens { get { return m_tokens; } }

		[JsonProperty]
		[BsonElement]
		private List<UserToken> m_tokens = new List<UserToken>();

		public void Add(User parent, UserToken token)
		{
			if(!token.Verify(out var error))
			{
				throw new Exception(error);
			}
			var type = token.GetType();
			var isSingleton = type.GetCustomAttribute<SingletonTokenAttribute>();
			if (isSingleton != null && Tokens.Any(action => action.GetType() == type))
			{
				throw new SingletonTokenDuplicateException($"Cannot have multiple {type.Name} Tokens");
			}
			m_tokens.Add(token);
			token.OnAdd(parent);
		}

		public bool RemoveAll<T>(User parent) where T: IRemovableToken
		{
			return Remove(parent, t => t is T);
		}

		public bool Remove(User parent, IRemovableToken token)
		{
			return Remove(parent, t => t.Guid == token.Guid);
		}

		public bool Remove(User parent, Guid tokenGuid)
		{
			return Remove(parent, t => t.Guid == tokenGuid);
		}

		public bool Remove(User parent, Func<IRemovableToken, bool> removeWhere)
		{
			var toRemove = Tokens.OfType<IRemovableToken>().Where(t => removeWhere(t));
			if(!toRemove.Any())
			{
				return false;
			}
			foreach(var token in toRemove)
			{
				token.OnRemove(parent);
			}
			m_tokens = m_tokens.Where(t1 => !toRemove.Any(t2 => t2.Guid == t1.Guid)).ToList();
			return true;
		}

		public T GetSingleToken<T>() where T:UserToken
		{
			return Tokens.SingleOrDefault(pa => pa is T) as T;
		}

		public T GetByGuid<T>(Guid gUID)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<T> GetTokens<T>() where T : UserToken => Tokens.OfType<T>();

		public T GetToken<T>(Guid guid) where T : UserToken => Tokens.SingleOrDefault(t => t.Guid == guid) as T;
	}

}
