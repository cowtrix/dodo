using System;
using System.Collections.Generic;
using Resources.Security;
using Dodo.Users;
using Resources;
using Resources.Serializers;
using Common.Security;
using Dodo.Users.Tokens;
using Common;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel;
using System.Security;
using Microsoft.AspNetCore.Mvc.Formatters.Internal;
using Dodo.Email;
using System.Reflection;

namespace Dodo
{
	public class GroupResourceReferenceSerializer : ResourceReferenceSerializer<GroupResource> { }

	public abstract class GroupResource :
		NotificationResource,
		IPublicResource,
		IGroupResource
	{
		public const string IS_MEMBER_AUX_TOKEN = "isMember";

		[View(EPermissionLevel.PUBLIC, customDrawer: "markdown", inputHint: IDescribedResource.MARKDOWN_INPUT_HINT)]
		[Name("Public Description")]
		[Resources.MaxStringLength]
		[ViewDrawer("html")]
		[PatchCallback(nameof(OnValueChanged))]
		public string PublicDescription { get; set; }

		[Name("Published")]
		[PatchCallback(nameof(OnPublished))]
		[View(EPermissionLevel.ADMIN, customDrawer: "published", priority: -1, inputHint: IPublicResource.PublishInputHint)]
		public bool IsPublished { get; set; }

		[BsonElement]
		protected UserStore Members { get; set; } = new UserStore();

		public GroupResource() : base() { }

		public GroupResource(AccessContext context, DescribedResourceSchemaBase schema) : base(context, schema)
		{
			PublicDescription = schema.PublicDescription;
		}

		#region Group
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM, customDrawer: "null")]
		public int MemberCount { get { return Members.Count; } }

		public bool IsMember(User user)
		{
			return Members.IsSubscribed(this, user);
		}

		public void Leave(AccessContext accessContext)
		{
			Members.Unsubscribe(this, accessContext);
			using var rscLock = new ResourceLock(accessContext.User);
			var user = rscLock.Value as User;
			if (!user.TokenCollection.Remove<UserJoinedGroupToken>(accessContext, EPermissionLevel.OWNER,
				t => t.Resource.Guid == Guid, user))
			{
				Logger.Error($"Failed to remove UserJoinedGroupToken from user {user} for {this}");
				return;
			}
			ResourceUtility.GetManager<User>().Update(user, rscLock);
		}

		public void Join(AccessContext accessContext)
		{
			Members.Subscribe(this, accessContext);
			using var rscLock = new ResourceLock(accessContext.User);
			var user = rscLock.Value as User;
			user.TokenCollection.AddOrUpdate(user, new UserJoinedGroupToken(this));
			ResourceUtility.GetManager<User>().Update(user, rscLock);
		}
		#endregion

		public override Passphrase GetPrivateKey(AccessContext context)
		{
			if (this is IOwnedResource owned)
			{
				return owned.Parent.GetValue<ITokenResource>().GetPrivateKey(context);
			}
			return default;
		}

		public override void AppendMetadata(Dictionary<string, object> view, EPermissionLevel permissionLevel,
			object requester, Passphrase passphrase)
		{
			var user = requester is ResourceReference<User> ? ((ResourceReference<User>)requester).GetValue() : requester as User;
			var context = new AccessContext(user, passphrase);
			view.Add(IS_MEMBER_AUX_TOKEN, IsMember(context.User));
			base.AppendMetadata(view, permissionLevel, requester, passphrase);
		}

		public void OnValueChanged(object requester, Passphrase passphrase, MemberInfo method, object oldValue, object newValue)
			=> UserEmailManager.RegisterUpdate(this, $"{method.Name} date was changed: {newValue}");

		public void OnPublished(object requester, Passphrase passphrase, MemberInfo method, object oldValue, object newValue)
		{
			if(!(this is IOwnedResource owned))
			{
				return;
			}
			UserEmailManager.RegisterUpdate(owned.Parent.GetValue<IPublicResource>(), 
				$"A new {GetType().GetName()} was created: {Name}");
		}

	}
}
