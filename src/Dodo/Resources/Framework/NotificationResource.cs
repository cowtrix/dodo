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
	public abstract class NotificationResource : DodoResource, INotificationResource
	{
		private const string METADATA_PUBLISHED = "published";
		public const string METADATA_NOTIFICATIONS_KEY = "notifications";
		public NotificationResource() : base() { }

		public NotificationResource(AccessContext creator, ResourceSchemaBase schema) : base(creator, schema)
		{
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
