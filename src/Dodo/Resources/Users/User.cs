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

namespace Dodo.Users
{
	public class User : DodoResource
	{
		public const string ADMIN_OF_KEY = "ADMIN_OF";
		public const string ROLES_HELD_KEY = "ROLES";
		public const string NOTIFICATIONS_KEY = "NOTIFICATIONS";

		#region Data
		[View(EPermissionLevel.USER)]
		[VerifyObject]
		public PersonalInfo PersonalData = new PersonalInfo();
		[View(EPermissionLevel.USER)]
		[VerifyObject]
		public AuthorizationData AuthData;
		public PushActionCollection PushActions = new PushActionCollection();
		#endregion

		public User() : base(default, default)
		{
		}

		public User(AccessContext context, UserSchema schema) : base(default, schema)
		{
			AuthData = new AuthorizationData(schema.Username, schema.Password);
			PersonalData.Email = schema.Email;
		}

		public override void AppendAuxilaryData(Dictionary<string, object> view, EPermissionLevel permissionLevel, object requester, Passphrase passphrase)
		{
			var requesterUser = requester is ResourceReference<User> ? ((ResourceReference<User>)requester).GetValue() : (User)requester;
			if(permissionLevel >= EPermissionLevel.ADMIN)
			{
				var accessContext = new AccessContext(requesterUser, passphrase);
				view.Add(ADMIN_OF_KEY, new
				{
					Rebellions = ResourceUtility.GetManager<Rebellion>().Get(r => r.IsAdmin(this, accessContext)).Select(r => r.GUID),
					WorkingGroups = ResourceUtility.GetManager<WorkingGroup>().Get(r => r.IsAdmin(this, accessContext)).Select(r => r.GUID),
					LocalGroups = ResourceUtility.GetManager<LocalGroup>().Get(r => r.IsAdmin(this, accessContext)).Select(r => r.GUID),
				});
				view.Add(ROLES_HELD_KEY, ResourceUtility.GetManager<Role>().Get(r => r.RoleHolders.IsAuthorised(this, passphrase)).Select(r => r.GUID));
			}
			if(permissionLevel == EPermissionLevel.OWNER)
			{
				view.Add(NOTIFICATIONS_KEY, PushActions.Actions.Select(x => new Notification { Message = x.GetNotificationMessage(), GUID = x.GUID })
					.Where(x => !string.IsNullOrEmpty(x.Message)).ToList());
			}
			base.AppendAuxilaryData(view, permissionLevel, requester, passphrase);
		}
	}
}
