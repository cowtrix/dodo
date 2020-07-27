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

namespace Dodo
{

	public class GroupResourceReferenceSerializer : ResourceReferenceSerializer<GroupResource> { }

	public abstract class GroupResource :
		NotificationResource,
		IPublicResource,
		IGroupResource
	{
		public const string IS_MEMBER_AUX_TOKEN = "isMember";

		[View(EPermissionLevel.PUBLIC, customDrawer: "markdown", inputHint:IDescribedResource.MARKDOWN_INPUT_HINT)]
		[Name("Public Description")]
		[Resources.MaxStringLength]
		[ViewDrawer("html")]
		public string PublicDescription { get; set; }

		[Name("Published")]
		[View(EPermissionLevel.ADMIN, customDrawer:"published", priority: -1, inputHint: IPublicResource.PublishInputHint)]
		public bool IsPublished { get; set; }

		[BsonElement]
		private SecureUserStore m_members { get; set; } = new SecureUserStore();

		public GroupResource() : base() { }

		public GroupResource(AccessContext context, DescribedResourceSchemaBase schema) : base(context, schema)
		{
			PublicDescription = schema.PublicDescription;
		}

		#region Group
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM, customDrawer:"null")]
		public int MemberCount { get { return m_members.Count; } }

		public bool IsMember(AccessContext context)
		{
			if (!context.Challenge())
			{
				return false;
			}
			return m_members.IsAuthorised(context);
		}

		public void Leave(AccessContext accessContext)
		{
			m_members.Remove(accessContext.User.CreateRef(), accessContext.Passphrase);
			using var rscLock = new ResourceLock(accessContext.User);
			var user = rscLock.Value as User;
			if(!user.TokenCollection.Remove<UserJoinedGroupToken>(accessContext, EPermissionLevel.OWNER, 
				t => t.Resource.Guid == Guid, user))
			{
				Logger.Error($"Failed to remove UserJoinedGroupToken from user {user} for {this}");
				return;
			}
			ResourceUtility.GetManager<User>().Update(user, rscLock);
		}

		public void Join(AccessContext accessContext)
		{
			m_members.Add(accessContext.User.CreateRef(), accessContext.Passphrase);
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
			view.Add(IS_MEMBER_AUX_TOKEN, IsMember(context) ? "true" : "false");
			base.AppendMetadata(view, permissionLevel, requester, passphrase);
		}
	}
}
