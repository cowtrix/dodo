using Common.Extensions;
using Dodo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Resources;
using System.Linq;
using System.Threading.Tasks;
using Dodo.SharedTest;
using Dodo.Users.Tokens;
using DodoTest.Framework.Postman;
using Newtonsoft.Json;
using Dodo.Models;
using System;
using Microsoft.AspNetCore.Routing.Patterns;

namespace RESTTests
{

	public abstract class GroupResourceTestBase<T, TSchema> : RESTTestBase<T, TSchema>
		where T : GroupResource
		where TSchema : DescribedResourceSchemaBase
	{
		#region Creation
		[TestMethod]
		[TestCategory("Creation")]
		public override async Task CanCreate()
		{
			var user = GetRandomUser(out var password, out var context);
			using (var rscLock = new ResourceLock(user))
			{
				user.TokenCollection.AddOrUpdate(user, new ResourceCreationToken(typeof(T)));
				UserManager.Update(user, rscLock);
			}
			await Login(user.Slug, password);
			var schema = SchemaGenerator.GetRandomSchema<T>(context) as TSchema;
			var response = await RequestJSON($"{Dodo.DodoApp.API_ROOT}{ResourceRoot}", EHTTPRequestType.POST, schema);
			user = UserManager.GetSingle(u => u.Guid == user.Guid);
			var rsc = ResourceManager.GetSingle(r => r.Guid.ToString() == response.Value<string>(nameof(IRESTResource.Guid).ToCamelCase()));
			VerifyCreatedObject(rsc, response, schema);

			Postman.Update(
				new PostmanEntryAddress { Category = PostmanCategory, Request = $"Create a new {PostmanTypeName}" },
				LastRequest);
		}

		[TestMethod]
		[TestCategory("Creation")]
		public async Task CreatorIsShownAsOwner()
		{
			var user = GetRandomUser(out var password, out var context);
			var group = CreateObject<T>(context);
			await Login(user.Slug, password);
			var obj = await RequestJSON($"{Dodo.DodoApp.API_ROOT}{ResourceRoot}/{group.Guid}", EHTTPRequestType.GET);
			Assert.AreEqual(PermissionLevel.OWNER,
				obj.Value<JObject>(Resource.METADATA).Value<string>(Resource.METADATA_PERMISSION));
		}
		#endregion

		#region Joining & Leaving
		[TestMethod]
		[TestCategory("Joining & Leaving")]
		public async Task CanJoin()
		{
			var group = CreateObject<T>();
			var user = GetRandomUser(out var password, out var context);
			await Login(user.Slug, password);
			var joinReq = await Request($"{Dodo.DodoApp.API_ROOT}{ResourceRoot}/{group.Guid}/join", EHTTPRequestType.POST);
			var verify = await RequestJSON($"{Dodo.DodoApp.API_ROOT}{ResourceRoot}/{group.Guid}", EHTTPRequestType.GET);
			Assert.AreEqual(verify[Resource.METADATA]["isMember"].Value<bool>(), true);
			/*var profile = await RequestJSON($"auth", EHTTPRequestType.GET);	// Membership no longer shown in user metadata
			var metadata = profile["metadata"];
			var memberOf = metadata["memberOf"];
			Assert.IsTrue(memberOf.Any(x => x["slug"].Value<string>() == group.Slug));*/
			Postman.Update(
				new PostmanEntryAddress { Category = PostmanCategory, Request = $"Join a {PostmanTypeName}" },
				joinReq);
		}

		[TestMethod]
		[TestCategory("Joining & Leaving")]
		public async Task CanLeave()
		{
			var group = CreateObject<T>();
			var user = GetRandomUser(out var password, out var context);
			await Login(user.Slug, password);
			await Request($"{Dodo.DodoApp.API_ROOT}{ResourceRoot}/{group.Guid}/join", EHTTPRequestType.POST);
			var leaveReq = await Request($"{Dodo.DodoApp.API_ROOT}{ResourceRoot}/{group.Guid}/leave", EHTTPRequestType.POST);
			var verify = await RequestJSON($"{Dodo.DodoApp.API_ROOT}{ResourceRoot}/{group.Guid}", EHTTPRequestType.GET);
			Assert.AreEqual(verify[Resource.METADATA]["isMember"].Value<bool>(), false);

			Postman.Update(
				new PostmanEntryAddress { Category = PostmanCategory, Request = $"Leave a {PostmanTypeName}" },
				leaveReq);
		}
		#endregion

		#region Notifications
		[TestMethod]
		[TestCategory("Notifications")]
		public async Task Notifications_CanGetNotifications()
		{
			var group = CreateObject<T>();
			var request = await RequestJSON<JObject>($"{Dodo.DodoApp.API_ROOT}{ResourceRoot}/notifications/{group.Slug}", EHTTPRequestType.GET);
			var notifications = request.Value<JArray>("notifications").Values<JToken>().Select(r => r.ToObject<Notification>());
			Assert.IsTrue(notifications.Any());

			Postman.Update(
				new PostmanEntryAddress { Category = PostmanCategory, Request = $"Get Notifications" }, LastRequest);
		}

		[TestMethod]
		[TestCategory("Notifications")]
		public async Task Notifications_AdminCanPostPublicNotification()
		{
			const string Message = "This is a test notification.";
			GetRandomUser(out var pass, out var con);
			var group = CreateObject<T>(con);

			// Create the notification
			await Login(con.User.Slug, pass);
			await RequestJSON<JObject>($"{Dodo.DodoApp.API_ROOT}{ResourceRoot}/notifications/{group.Slug}/new", EHTTPRequestType.POST,
				new NotificationModel { Message = Message, PermissionLevel = EPermissionLevel.PUBLIC });
			await Logout();

			// Anon user should see it
			var request = await RequestJSON<JObject>($"{Dodo.DodoApp.API_ROOT}{ResourceRoot}/notifications/{group.Slug}", EHTTPRequestType.GET);
			var notifications = request.Value<JArray>("notifications").Values<JToken>().Select(r => r.ToObject<Notification>());
			Assert.IsTrue(notifications.Any());
			var n = notifications.SingleOrDefault(n => n.Message == Message);
			Assert.IsNotNull(n);
			Assert.AreEqual(Message, n.Message);
		}

		[TestMethod]
		[TestCategory("Notifications")]
		public async Task Notifications_AdminCanViewAdminNotification()
		{
			const string Message = "This is a test notification.";
			GetRandomUser(out var pass, out var con);
			var group = CreateObject<T>(con);

			// Create the notification
			await Login(con.User.Slug, pass);
			await RequestJSON<JObject>($"{Dodo.DodoApp.API_ROOT}{ResourceRoot}/notifications/{group.Slug}/new", EHTTPRequestType.POST,
				new NotificationModel { Message = Message, PermissionLevel = EPermissionLevel.ADMIN });
			var request = await RequestJSON<JObject>($"{Dodo.DodoApp.API_ROOT}{ResourceRoot}/notifications/{group.Slug}", EHTTPRequestType.GET);
			var notifications = request.Value<JArray>("notifications").Values<JToken>().Select(r => r.ToObject<Notification>());
			Assert.IsTrue(notifications.Any());
			var n = notifications.SingleOrDefault(n => n.Message == Message);
			Assert.IsNotNull(n);
			Assert.AreEqual(Message, n.Message);
		}

		[TestMethod]
		[TestCategory("Notifications")]
		public async Task Notifications_AdminCanDeleteNotification()
		{
			const string Message = "This is a test notification.";
			GetRandomUser(out var pass, out var con);
			var group = CreateObject<T>(con);
			Guid notGuid;
			// Create a notification
			using (var rscLock = new ResourceLock(group))
			{
				var token = new SimpleNotificationToken(con.User, "Test", Message, null, ENotificationType.Announcement, EPermissionLevel.PUBLIC, true);
				notGuid = token.Guid;
				group.TokenCollection.AddOrUpdate(group, token);
				ResourceManager.Update(group, rscLock);
			}

			// Delete the notification
			await Login(con.User.Slug, pass);
			await Request($"{Dodo.DodoApp.API_ROOT}{ResourceRoot}/notifications/{group.Slug}/delete?notification={notGuid}", EHTTPRequestType.POST);

			group = ResourceManager.GetSingle(r => r.Guid == group.Guid) as T;
			var n = group.GetNotifications(con, EPermissionLevel.PUBLIC).SingleOrDefault(n => n.Message == Message);
			Assert.IsNull(n);
		}

		[TestMethod]
		[TestCategory("Notifications")]
		public async Task Notifications_AnonCannotViewAdminNotification()
		{
			const string Message = "This is a test notification.";
			GetRandomUser(out var pass, out var con);
			var group = CreateObject<T>(con);

			// Create the notification
			await Login(con.User.Slug, pass);
			await RequestJSON<JObject>($"{Dodo.DodoApp.API_ROOT}{ResourceRoot}/notifications/{group.Slug}/new", EHTTPRequestType.POST,
				new NotificationModel { Message = Message, PermissionLevel = EPermissionLevel.ADMIN });
			await Logout();
			var request = await RequestJSON<JObject>($"{Dodo.DodoApp.API_ROOT}{ResourceRoot}/notifications/{group.Slug}", EHTTPRequestType.GET);
			var notifications = request.Value<JArray>("notifications").Values<JToken>().Select(r => r.ToObject<Notification>());
			Assert.IsTrue(notifications.Any());
			var n = notifications.SingleOrDefault(n => n.Message == Message);
			Assert.IsNull(n);
		}
		#endregion
	}
}
