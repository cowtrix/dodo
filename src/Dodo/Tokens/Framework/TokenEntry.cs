using Common;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
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
		public HashedResourceReference Owner;
		public Guid Guid;
		public bool Removed;

		[BsonIgnore]
		public Type Type => Type.GetType(m_typeName);
		public EPermissionLevel PermissionLevel { get; set; }
		[BsonElement]
		private string m_typeName;

		public override string ToString() => $"Token [{m_typeName}]";

		public TokenEntry(ITokenResource owner, IToken token)
		{
			if (token == null || owner == null)
			{
				throw new ArgumentNullException();
			}
			Owner = new HashedResourceReference(owner, token.Guid.ToString());
			Guid = token.Guid;
			PermissionLevel = token.GetVisibility();
			m_typeName = token.GetType().FullName;
			if (!BsonClassMap.IsClassMapRegistered(token.GetType()))
			{
				Logger.Debug($"Registered BsonClassMap for {token.GetType()}");
				BsonClassMap.RegisterClassMap(new BsonClassMap(token.GetType()));
			}
			SetData(owner, token);
		}

		public abstract IToken GetToken(Passphrase privateKey);

		public IToken GetToken(AccessContext context)
		{
			if (context.User == null)
			{
				return null;
			}
			return GetToken(new Passphrase(context.User.AuthData.PrivateKey.GetValue(context.Passphrase)));
		}

		public abstract void SetData(ITokenResource owner, IToken token);
	}
}
