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
using System;

namespace Dodo.Users
{
	[DontCache]
	public class User : NotificationResource, IVerifiable, ITokenResource, INotificationResource
	{
		public const string ADMIN_OF_KEY = "adminOf";
		public const string MEMBER_OF_KEY = "memberOf";
		public const string ROLES_HELD_KEY = "roles";

		#region Data
		[View(EPermissionLevel.USER)]
		[VerifyObject]
		public PersonalInfo PersonalData { get; set; } = new PersonalInfo();
		[VerifyObject]
		public AuthorizationData AuthData { get; set; }
		[BsonIgnore]
		public override Passphrase PublicKey => new Passphrase(AuthData.PublicKey);
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
			/*var requesterUser = requester is ResourceReference<User> ? ((ResourceReference<User>)requester).GetValue() : (User)requester;
			var accessContext = new AccessContext(requesterUser, passphrase);
			if (permissionLevel == EPermissionLevel.OWNER)
			{
				view.Add(ADMIN_OF_KEY, TokenCollection.GetAllTokens<UserAddedAsAdminToken>(accessContext, EPermissionLevel.OWNER, this)
					.Select(r => r.Resource.GenerateJsonView(permissionLevel, requester, passphrase)).ToList());

				view.Add(MEMBER_OF_KEY, TokenCollection.GetAllTokens<UserJoinedGroupToken>(accessContext, EPermissionLevel.OWNER, this)
					.Select(r => r.Resource.GenerateJsonView(permissionLevel, requester, passphrase)).ToList());
			}*/
			base.AppendMetadata(view, permissionLevel, requester, passphrase);
		}

		public IEnumerable<Notification> GetNotifications(AccessContext accessContext, EPermissionLevel permissionLevel)
		{
			return TokenCollection.GetNotifications(accessContext, permissionLevel, this);
		}

		public bool DeleteNotification(AccessContext context, EPermissionLevel permissionLevel, Guid notification)
		{
			return TokenCollection.Remove<INotificationToken>(context, permissionLevel, notification, this);
		}

		public override Passphrase GetPrivateKey(AccessContext accessContext)
		{
			if (accessContext.User == null)
			{
				return default;
			}
			return new Passphrase(accessContext.User.AuthData.PrivateKey.GetValue(accessContext.Passphrase));
		}
	}
}
