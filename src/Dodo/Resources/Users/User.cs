using System.Collections.Generic;
using System.Linq;
using Common.Extensions;
using Resources.Security;
using Dodo.LocalGroups;
using Dodo.Rebellions;
using Dodo.Roles;
using Dodo.WorkingGroups;
using Resources;
using System.Security.Principal;
using Dodo.Users.Tokens;
using MongoDB.Bson.Serialization.Attributes;

namespace Dodo.Users
{
	public class User : DodoResource, IVerifiable, ITokenResource, INotificationResource
	{
		public const string ADMIN_OF_KEY = "adminOf";
		public const string ROLES_HELD_KEY = "roles";

		#region Data
		[View(EPermissionLevel.USER)]
		[VerifyObject]
		public PersonalInfo PersonalData = new PersonalInfo();
		[View(EPermissionLevel.USER)]
		[VerifyObject]
		public AuthorizationData AuthData;
		public TokenCollection TokenCollection = new TokenCollection();
		[BsonIgnore]
		public string PublicKey => AuthData.PublicKey;
		#endregion

		public User() : base()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context">Unused variable required due to this constructor being called by the Activator class</param>
		/// <param name="schema"></param>
		public User(AccessContext context, UserSchema schema) : base(default, schema)
		{
			AuthData = new AuthorizationData(schema.Password);
			PersonalData.Email = schema.Email;
			Slug = schema.Username;
		}

		public override bool VerifyExplicit(out string error)
		{
			var um = ResourceUtility.GetManager<User>();
			if (um.GetSingle(u => u.Slug == Slug && u.Guid != Guid) != null)
			{
				error = "A user with that username already exists";
				return false;
			}
			if (um.GetSingle(u => u.PersonalData.Email == PersonalData.Email && u.Guid != Guid) != null)
			{
				error = "A user with that email already exists";
				return false;
			}
			error = null;
			return true;
		}

		public override void AppendMetadata(Dictionary<string, object> view, EPermissionLevel permissionLevel, object requester, Passphrase passphrase)
		{
			var requesterUser = requester is ResourceReference<User> ? ((ResourceReference<User>)requester).GetValue() : (User)requester;
			var accessContext = new AccessContext(requesterUser, passphrase);
			if (permissionLevel >= EPermissionLevel.ADMIN)
			{
				view.Add(ADMIN_OF_KEY, new
				{
					Rebellions = ResourceUtility.GetManager<Rebellion>().Get(r => r.IsAdmin(this, accessContext)).Select(r => r.Guid),
					WorkingGroups = ResourceUtility.GetManager<WorkingGroup>().Get(r => r.IsAdmin(this, accessContext)).Select(r => r.Guid),
					LocalGroups = ResourceUtility.GetManager<LocalGroup>().Get(r => r.IsAdmin(this, accessContext)).Select(r => r.Guid),
				});
				view.Add(ROLES_HELD_KEY, ResourceUtility.GetManager<Role>().Get(r => r.RoleHolders.IsAuthorised(this, passphrase)).Select(r => r.Guid));
			}
			base.AppendMetadata(view, permissionLevel, requester, passphrase);
		}

		public IEnumerable<Notification> GetNotifications(AccessContext accessContext, EPermissionLevel permissionLevel)
		{
			return TokenCollection.GetNotifications(accessContext, 
				new Passphrase(accessContext.User.AuthData.PrivateKey.GetValue(accessContext.Passphrase)), permissionLevel);
		}

		public void AddToken(IToken token, EPermissionLevel permissionLevel)
		{
			TokenCollection.Add(this, token, permissionLevel);
		}
	}
}
