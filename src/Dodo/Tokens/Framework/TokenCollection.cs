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
using Common;

namespace Dodo.Users.Tokens
{
	public class ITokenOwnerSerializer : ResourceReferenceSerializer<ITokenResource> { }

	/// <summary>
	/// This collection enforces some restrictions on tokens, e.g. there can only be
	/// one instance of some tokens in a list
	/// </summary>
	public class TokenCollection
	{
		public List<Notification> GetNotifications(AccessContext accessContext, EPermissionLevel permissionLevel, ITokenResource parent) => 
			GetAllTokens<INotificationToken>(accessContext, permissionLevel, parent)
					.Select(x => x.GetNotification(accessContext))
					.Where(x => !string.IsNullOrEmpty(x?.Message))
					.OrderByDescending(x => x.Timestamp).ToList();

		[BsonElement]
		private List<TokenEntry> m_tokens = new List<TokenEntry>();
		[BsonIgnore]
		public int Count => m_tokens.Count;
		public IToken AddOrUpdate(ITokenResource parent, IToken token)
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
			if(token.Guid == default)
			{
				throw new Exception($"Token type {token.GetType()} is not falling through to the base .OnAdd()");
			}
			var entry = m_tokens.SingleOrDefault(e => e.Guid == token.Guid);
			var isSingleton = type.GetCustomAttribute<SingletonTokenAttribute>();
			if (isSingleton != null && HasToken(token.GetType(), entry?.Guid))
			{
				throw new SingletonTokenDuplicateException($"Cannot have multiple {type.Name} Tokens");
			}
			token.OnAdd(parent);
			if (entry == null)
			{
				entry = token.Encrypted ? (TokenEntry)new EncryptedTokenEntry(parent, token) : (TokenEntry)new PlainTokenEntry(parent, token);
				m_tokens.Add(entry);
			}
			else
			{
				entry.SetData(parent, token);
			}
			return token;
		}

		public bool RemoveAllOfType<T>(AccessContext context, EPermissionLevel permissionLevel, ITokenResource parent) 
			where T: class, IToken
		{
			return Remove<T>(context, permissionLevel, t => t is T, parent);
		}

		public bool Remove<T>(AccessContext context, EPermissionLevel permissionLevel, IRemovableToken token, ITokenResource parent)
			where T : class, IToken
		{
			return Remove<T>(context, permissionLevel, t => t.Guid == token.Guid, parent);
		}

		public bool Remove<T>(AccessContext context, EPermissionLevel permissionLevel, Guid tokenGuid, ITokenResource parent)
			where T : class, IToken
		{
			return Remove<T>(context, permissionLevel, t => t.Guid == tokenGuid, parent);
		}

		public bool Remove<T>(AccessContext context, EPermissionLevel permissionLevel, Func<T, bool> removeWhere, ITokenResource parent)
			where T:class, IToken
		{
			var toRemove = GetAllTokens<T>(context, permissionLevel, parent)
				.Where(t => removeWhere(t)).ToList();
			if (!toRemove.Any())
			{
				return false;
			}
			foreach (var token in toRemove.OfType<IRemovableToken>())
			{
				token.OnRemove(context);				
			}
			m_tokens.RemoveAll(t => toRemove.Any(r => t.Guid == r.Guid));
			return true;
		}

		public bool HasToken<T>() where T: class, IToken
		{
			return HasToken(typeof(T));
		}

		public bool HasToken(Type t, Guid? exception = null)
		{
			if(exception != null)
			{
				return m_tokens.Any(mt => mt.Guid != exception && mt.Type.IsAssignableFrom(t));
			}
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
				IToken t = null;
				try
				{
					t = token.GetToken(pk);
				}
				catch(Exception e)
				{
					Logger.Exception(e);
				}
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
