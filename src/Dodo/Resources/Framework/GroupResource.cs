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
		DodoResource,
		IPublicResource,
		IGroupResource
	{
		public const string IS_MEMBER_AUX_TOKEN = "isMember";

		/// <summary>
		/// This is a MarkDown formatted, public facing description of this resource
		/// </summary>
		[View(EPermissionLevel.PUBLIC, customDrawer: "markdown")]
		[Name("Public Description")]
		[Resources.MaxStringLength]
		[ViewDrawer("html")]
		public string PublicDescription { get; set; }

		[Name("Published")]
		[View(EPermissionLevel.ADMIN, priority: -1, inputHint: IPublicResource.PublishInputHint)]
		public bool IsPublished { get; set; }

		[BsonElement]
		private SecureUserStore m_members { get; set; } = new SecureUserStore();

		public GroupResource() : base() { }

		public GroupResource(AccessContext context, DescribedResourceSchemaBase schema) : base(context, schema)
		{
		}

		#region Group
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
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
		}

		public void Join(AccessContext accessContext)
		{
			m_members.Add(accessContext.User.CreateRef(), accessContext.Passphrase);
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
