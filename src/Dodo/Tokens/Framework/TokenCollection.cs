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
using Resources.Serializers;

namespace Dodo.Users.Tokens
{
	public class ITokenOwnerSerializer : ResourceReferenceSerializer<ITokenResource> { }

	public interface ITokenResource : IRESTResource
	{
		string PublicKey { get; }
		void AddToken(IToken token);
		Passphrase GetPrivateKey(AccessContext context);
	}

	/// <summary>
	/// This collection enforces some restrictions on tokens, e.g. there can only be
	/// one instance of some tokens in a list
	/// </summary>
	public class TokenCollection
	{
		public List<Notification> GetNotifications(AccessContext accessContext, EPermissionLevel permissionLevel, ITokenResource parent) => 
			GetAllTokens<INotificationToken>(accessContext, permissionLevel, parent)
					.Select(x => x.GetNotification(accessContext))
					.Where(x => !string.IsNullOrEmpty(x.Message))
					.OrderByDescending(x => x.Timestamp).ToList();

		[BsonElement]
		private List<TokenEntry> m_tokens = new List<TokenEntry>();
		[BsonIgnore]
		public int Count => m_tokens.Count;
		public void AddOrUpdate(ITokenResource parent, IToken token)
		{
			if(token == null)
			{
				throw new ArgumentNullException();
			}
			if(token.Guid == default)
			{
				throw new ArgumentException("Token Guid cannot be null - make sure base constructor is being called");
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
			var entry = m_tokens.SingleOrDefault(e => e.Guid == token.Guid);
			if(entry == null)
			{
				entry = token.Encrypted ? (TokenEntry)new EncryptedTokenEntry(parent, token) : (TokenEntry)new PlainTokenEntry(parent, token);
				m_tokens.Add(entry);
			}
			else
			{
				entry.SetData(parent, token);
			}
		}

		public void Update(ITokenResource parent, IToken token)
		{
			

		}

		public bool RemoveAll<T>(AccessContext context, EPermissionLevel permissionLevel, ITokenResource parent) where T: IRemovableToken
		{
			return Remove(context, permissionLevel, t => t is T, parent);
		}

		public bool Remove(AccessContext context, EPermissionLevel permissionLevel, IRemovableToken token, ITokenResource parent)
		{
			return Remove(context, permissionLevel, t => t.Guid == token.Guid, parent);
		}

		public bool Remove(AccessContext context, EPermissionLevel permissionLevel, Guid tokenGuid, ITokenResource parent)
		{
			return Remove(context, permissionLevel, t => t.Guid == tokenGuid, parent);
		}

		public bool Remove(AccessContext context, EPermissionLevel permissionLevel, Func<IRemovableToken, bool> removeWhere, ITokenResource parent)
		{
			var toRemove = GetAllTokens<IRemovableToken>(context, permissionLevel, parent).Where(t => removeWhere(t));
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

		public T GetSingleToken<T>(AccessContext context, EPermissionLevel permissionLevel, ITokenResource parent) where T: class, IToken
		{
			return GetSingleToken(context, typeof(T), permissionLevel, parent) as T;
		}

		public IToken GetSingleToken(AccessContext context, Type tokenType, EPermissionLevel permissionLevel, ITokenResource parent)
		{
			return GetAllTokens(context, permissionLevel, parent)
				.SingleOrDefault(pa => tokenType.IsAssignableFrom(pa.GetType()));
		}

		public IEnumerable<T> GetAllTokens<T>(AccessContext context, EPermissionLevel permissionLevel, ITokenResource parent) where T : class, IToken
		{
			return GetAllTokens(context, permissionLevel, parent).OfType<T>();
		}

		public IEnumerable<IToken> GetAllTokens(AccessContext context, EPermissionLevel permissionLevel, ITokenResource parent)
		{
			var pk = parent != null ? parent.GetPrivateKey(context) : default;
			foreach (var token in m_tokens.Where(t => t.PermissionLevel <= permissionLevel))
			{
				var t = token.GetToken(pk);
				if (t != null)
				{
					yield return t;
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
