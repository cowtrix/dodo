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

namespace Dodo.Users
{
	public class User : DodoResource, IVerifiable
	{
		public const string ADMIN_OF_KEY = "adminOf";
		public const string ROLES_HELD_KEY = "roles";
		public const string NOTIFICATIONS_KEY = "notifications";

		#region Data
		[View(EPermissionLevel.USER)]
		[VerifyObject]
		public PersonalInfo PersonalData = new PersonalInfo();
		[View(EPermissionLevel.USER)]
		[VerifyObject]
		public AuthorizationData AuthData;
		public TokenCollection TokenCollection = new TokenCollection();
		#endregion

		public User() : base(default, default)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context">Unused variable required due to this constructor being called by the Activator class</param>
		/// <param name="schema"></param>
		public User(AccessContext context, UserSchema schema) : base(default, schema)
		{
			AuthData = new AuthorizationData(schema.Username, schema.Password);
			PersonalData.Email = schema.Email;
		}

		public override bool VerifyExplicit(out string error)
		{
			var um = ResourceUtility.GetManager<User>();
			if (um.GetSingle(u => u.AuthData.Username == AuthData.Username && u.Guid != Guid) != null)
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
			if(permissionLevel == EPermissionLevel.OWNER)
			{
				var notifications = TokenCollection.GetAllTokens<INotificationToken>(accessContext)
					.Select(x => new Notification { Message = x.GetNotification(accessContext), GUID = x.Guid })
					.Where(x => !string.IsNullOrEmpty(x.Message)).ToList();
				view.Add(NOTIFICATIONS_KEY, notifications);
			}
			base.AppendMetadata(view, permissionLevel, requester, passphrase);
		}
	}
}
