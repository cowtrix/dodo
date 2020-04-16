using Common;
using Common.Extensions;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Resources;
using Resources.Security;
using System;

namespace Dodo.Users.Tokens
{
	[BsonDiscriminator(RootClass = true)]
	[BsonKnownTypes(
		typeof(PlainTokenEntry),
		typeof(EncryptedTokenEntry)
		)]
	public abstract class TokenEntry
	{
		public ResourceReference<User> Owner;
		public Guid Guid;
		public bool Removed;

		[BsonIgnore]
		public Type Type => Type.GetType(m_typeName);
		[BsonElement]
		private string m_typeName;

		public TokenEntry(User targetUser, UserToken token)
		{
			if(token == null || targetUser == null)
			{
				throw new ArgumentNullException();
			}
			Owner = targetUser;
			Guid = token.Guid;
			m_typeName = token.GetType().FullName;
			if (!BsonClassMap.IsClassMapRegistered(token.GetType()))
			{
				Logger.Debug($"Registered BsonClassMap for {token.GetType()}");
				BsonClassMap.RegisterClassMap(new BsonClassMap(token.GetType()));
			}
		}

		public abstract UserToken GetToken(AccessContext context);
	}

	public class PlainTokenEntry : TokenEntry
	{
		[BsonElement]
		[JsonProperty]
		private string m_token { get; set; }

		public PlainTokenEntry(User targetUser, UserToken token) : base(targetUser, token)
		{
			m_token = JsonConvert.SerializeObject(token, JsonExtensions.StorageSettings);
		}

		public override UserToken GetToken(AccessContext context) => JsonConvert.DeserializeObject(m_token, JsonExtensions.StorageSettings) as UserToken;
	}

	public class EncryptedTokenEntry : TokenEntry
	{
		[BsonElement]
		private AsymmEncryptedStore<UserToken> m_data;

		public EncryptedTokenEntry(User targetUser, UserToken token) : base(targetUser, token)
		{
			m_data = new AsymmEncryptedStore<UserToken>(token, new Passphrase(targetUser.AuthData.PublicKey));
		}

		public override UserToken GetToken(AccessContext context)
		{
			if(context.User == null)
			{
				return null;
			}
			var pk = context.User.AuthData.PrivateKey.GetValue(context.Passphrase);
			return m_data.GetValue(pk);
		}
	}
}
