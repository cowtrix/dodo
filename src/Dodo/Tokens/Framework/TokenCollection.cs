using Resources.Security;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Resources;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using Common.Extensions;
using Common.Security;

namespace Dodo.Users.Tokens
{
	public interface ITokenOwner : IRESTResource
	{
		string PublicKey { get; }
	}

	/// <summary>
	/// This collection enforces some restrictions on tokens, e.g. there can only be
	/// one instance of some tokens in a list
	/// </summary>
	public class TokenCollection
	{
		public List<Notification> GetNotifications(AccessContext accessContext, Passphrase privateKey, EPermissionLevel permissionLevel) => 
			GetAllTokens<INotificationToken>(privateKey, permissionLevel)
					.Select(x => x.GetNotification(accessContext))
					.Where(x => !string.IsNullOrEmpty(x.Message)).ToList();

		[BsonElement]
		private List<TokenEntry> m_tokens = new List<TokenEntry>();
		[BsonIgnore]
		public int Count => m_tokens.Count;

		public void Add<T>(ITokenOwner parent, T token, EPermissionLevel permissionLevel = EPermissionLevel.OWNER) where T: Token
		{
			if(token == null)
			{
				throw new ArgumentNullException();
			}
			if(!token.Verify(out var error))
			{
				throw new Exception(error);
			}
			var type = token.GetType();
			var isSingleton = type.GetCustomAttribute<SingletonTokenAttribute>();
			if (isSingleton != null && HasToken(token.GetType()))
			{
				throw new SingletonTokenDuplicateException($"Cannot have multiple {type.Name} Tokens");
			}
			token.OnAdd(parent);
			if(token.Guid == default)
			{
				throw new Exception($"Token type {token.GetType()} is not falling through to the base .OnAdd()");
			}
			var newEntry = token.Encrypted ? (TokenEntry)new EncryptedTokenEntry(parent, token, permissionLevel) : (TokenEntry)new PlainTokenEntry(parent, token, permissionLevel);
			m_tokens.Add(newEntry);
		}

		public bool RemoveAll<T>(AccessContext context) where T: IRemovableToken
		{
			return Remove(context, t => t is T);
		}

		public bool Remove(AccessContext context, IRemovableToken token)
		{
			return Remove(context, t => t.Guid == token.Guid);
		}

		public bool Remove(AccessContext context, Guid tokenGuid)
		{
			return Remove(context, t => t.Guid == tokenGuid);
		}

		public bool Remove(AccessContext context, Func<IRemovableToken, bool> removeWhere)
		{
			var toRemove = GetAllTokens<IRemovableToken>(context, EPermissionLevel.OWNER).Where(t => removeWhere(t));
			if(!toRemove.Any())
			{
				return false;
			}
			foreach(var token in toRemove)
			{
				token.OnRemove(context);
			}
			m_tokens = m_tokens.Where(t1 => !toRemove.Any(t2 => t2.Guid == t1.Guid)).ToList();
			return true;
		}

		public bool HasToken<T>() where T: class, IToken
		{
			return HasToken(typeof(T));
		}

		public bool HasToken(Type t)
		{
			return m_tokens.Any(mt => mt.Type.IsAssignableFrom(t));
		}

		public T GetSingleToken<T>(AccessContext context) where T: class, IToken
		{
			return GetSingleToken(context, typeof(T)) as T;
		}

		public IToken GetSingleToken(AccessContext context, Type tokenType)
		{
			return GetAllTokens(context).SingleOrDefault(pa => tokenType.IsAssignableFrom(pa.GetType()));
		}

		public IEnumerable<T> GetAllTokens<T>(AccessContext context, EPermissionLevel permissionLevel) where T : class, IToken
		{
			return GetAllTokens<T>(new Passphrase(context.User.AuthData.PrivateKey.GetValue(context.Passphrase)), permissionLevel);
		}

		public IEnumerable<T> GetAllTokens<T>(Passphrase privateKey, EPermissionLevel permissionLevel) where T : class, IToken
		{
			foreach (var token in m_tokens.Where(t => t.PermissionLevel <= permissionLevel && typeof(T).IsAssignableFrom(t.Type)))
			{
				yield return token.GetToken(privateKey) as T;
			}
		}

		public IEnumerable<IToken> GetAllTokens(AccessContext context)
		{
			foreach (var entry in m_tokens)
			{
				var token = entry.GetToken(context);
				if(token != null)
				{
					yield return token;
				}
			}
		}

		public T GetToken<T>(AccessContext context, Guid guid) where T : class, IToken
		{
			return GetToken(context, guid) as T;
		}

		public IToken GetToken(AccessContext context, Guid guid)
		{
			var token = m_tokens.SingleOrDefault(mt => mt.Guid == guid);
			var pk = context.User.AuthData.PrivateKey.GetValue(context.Passphrase);
			if (token != null)
			{
				return token.GetToken(context);
			}
			return null;
		}
	}

}
