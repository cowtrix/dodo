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
		public ResourceReference<ITokenResource> Owner;
		public Guid Guid;
		public bool Removed;

		[BsonIgnore]
		public Type Type => Type.GetType(m_typeName);
		public EPermissionLevel PermissionLevel { get; set; }
		[BsonElement]
		private string m_typeName;

		public TokenEntry(ITokenResource owner, IToken token, EPermissionLevel permissionLevel)
		{
			if(token == null || owner == null)
			{
				throw new ArgumentNullException();
			}
			Owner = new ResourceReference<ITokenResource>(owner);
			Guid = token.Guid;
			PermissionLevel = permissionLevel;
			m_typeName = token.GetType().FullName;
			if (!BsonClassMap.IsClassMapRegistered(token.GetType()))
			{
				Logger.Debug($"Registered BsonClassMap for {token.GetType()}");
				BsonClassMap.RegisterClassMap(new BsonClassMap(token.GetType()));
			}
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
	}
}
