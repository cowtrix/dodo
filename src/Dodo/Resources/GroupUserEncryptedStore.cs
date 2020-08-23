using Common.Security;
using Dodo.Users;
using Dodo.Users.Tokens;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Resources;
using Resources.Security;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;

namespace Dodo.Roles
{
	/// <summary>
	/// With this class we want to allow a single user AND any administrators of a resource to
	/// be able to access a
	/// </summary>
	/// <typeparam name="ResourceReference<User>"></typeparam>
	/// <typeparam name="RoleApplicationData"></typeparam>
	public class GroupUserEncryptedStore : MultiSigEncryptedStore<ResourceReference<IAsymmCapableResource>, RoleApplicationData>
	{
		const int KEY_SIZE = 128;

		[JsonProperty]
		[BsonElement]
		private Resources.Security.AsymmEncryptedStore<string> m_groupPass;

		public GroupUserEncryptedStore(RoleApplicationData data, IAsymmCapableResource group, AccessContext context) : 
			base(data, context.User.CreateRef<IAsymmCapableResource>(), context.Passphrase)
		{
			var groupKey = new Passphrase(KeyGenerator.GetUniqueKey(KEY_SIZE));
			var pub = group.PublicKey;
			m_groupPass = new AsymmEncryptedStore<string>(groupKey.Value, pub);
			AddPermission(context.User.CreateRef<IAsymmCapableResource>(), context.Passphrase, group.CreateRef(), groupKey);
		}
	}
}
