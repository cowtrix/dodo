using Resources.Security;
using Resources;
using Newtonsoft.Json;
using Common.Extensions;
using Common;
using System;
using System.Collections.Generic;
using Resources.Location;
using Dodo.Users.Tokens;
using MongoDB.Bson.Serialization.Attributes;

namespace Dodo
{
	public abstract class DodoResource : Resource, INotificationResource
	{
		private const string METADATA_PUBLISHED = "published";
		public const string METADATA_NOTIFICATIONS_KEY = "notifications";
		public DodoResource() : base() { }

		public DodoResource(AccessContext creator, ResourceSchemaBase schema) : base(schema)
		{
			if (creator.User != null)
			{
				Creator = SecurityExtensions.GenerateID(creator.User, creator.Passphrase);
			}
		}

		public string Creator { get; private set; }

		public bool IsCreator(AccessContext context)
		{
			if (context.User == null)
			{
				return false;
			}
			return Creator == SecurityExtensions.GenerateID(context.User, context.Passphrase);
		}

		public virtual void OnDestroy()
		{
			if (this is IOwnedResource owned && owned.Parent.HasValue())
			{
				// Remove listing from parent resource if needed				
				using var rscLock = new ResourceLock(owned.Parent.Guid);
				{
					var parent = rscLock.Value as AdministratedGroupResource;
					if (!parent.RemoveChild(owned))
					{
						throw new Exception($"Unexpectedly failed to remove child object {Guid} from parent resource");
					}
					ResourceUtility.GetManager(parent.GetType()).Update(parent, rscLock);
				}
			}
		}

		#region Notifications & Tokens
		[BsonElement]
		public TokenCollection TokenCollection { get; private set; } = new TokenCollection();

		public IEnumerable<Notification> GetNotifications(AccessContext context, EPermissionLevel permissionLevel)
		{
			return TokenCollection.GetNotifications(context, permissionLevel, this);
		}

		public bool DeleteNotification(AccessContext context, EPermissionLevel permissionLevel, Guid notification)
		{
			return TokenCollection.Remove(context, permissionLevel, notification, this);
		}

		public abstract Passphrase GetPrivateKey(AccessContext context);
		[BsonElement]
		[View(EPermissionLevel.MEMBER, EPermissionLevel.SYSTEM, customDrawer: "null")]
		public virtual string PublicKey
		{
			get
			{
				if (this is IOwnedResource owned)
				{
					return owned.Parent.GetValue<ITokenResource>().PublicKey;
				}
				return null;
			}
		}
		#endregion
	}
}
