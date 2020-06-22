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
using Dodo.Users;
using Dodo.Models;
using System;

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
				user.TokenCollection.Add(user, new ResourceCreationToken(typeof(T)));
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

		#region Administration
		[TestMethod]
		[TestCategory("Administration")]
		public async Task CanAddAdminFromGuid()
		{
			// Create a new user
			var user1 = GetRandomUser(out var user1Password, out var user1Context);
			// Let them create a new group
			var group = CreateObject<T>(user1Context);
			Assert.IsTrue(group.IsAdmin(user1, user1Context));
			await Login(user1.Slug, user1Password);

			var user2 = GetRandomUser(out var user2Password, out var user2Context);
			var apiReq = await Request($"{Dodo.DodoApp.API_ROOT}{ResourceRoot}/{group.Guid}/addadmin",
				EHTTPRequestType.POST, user2.Guid);

			var obj = await RequestJSON($"{Dodo.DodoApp.API_ROOT}{ResourceRoot}/{group.Guid}", EHTTPRequestType.GET);
			Assert.IsNotNull(obj.Value<JObject>(nameof(GroupResource.AdministratorData).ToCamelCase()).Value<JArray>(nameof(GroupResource.AdminData.Administrators).ToCamelCase()).Values<JToken>()
				.Single(s => s.Value<string>(nameof(IResourceReference.Guid).ToCamelCase()).ToString() == user2.Guid.ToString()));
			await Logout();

			await Login(user2.Slug, user2Password);
			obj = await RequestJSON($"{Dodo.DodoApp.API_ROOT}{ResourceRoot}/{group.Guid}", EHTTPRequestType.GET);
			Assert.AreEqual(PermissionLevel.ADMIN,
				obj.Value<JObject>(Resource.METADATA).Value<string>(Resource.METADATA_PERMISSION));

			Postman.Update(
				new PostmanEntryAddress { Category = PostmanCategory, Request = $"Add an Administrator by Guid" },
				apiReq);
		}

		[TestMethod]
		[TestCategory("Administration")]
		public async Task CanAddAdminFromEmail()
		{
			Assert.Inconclusive();
			// Create a new user
			var user1 = GetRandomUser(out var user1Password, out var user1Context);
			// Let them create a new group
			var group = CreateObject<T>(user1Context);
			Assert.IsTrue(group.IsAdmin(user1, user1Context));
			await Login(user1.Slug, user1Password);

			var user2Email = "myUser2@email.com";
			var apiReq = await Request($"{Dodo.DodoApp.API_ROOT}{ResourceRoot}/{group.Guid}/addadmin",
				EHTTPRequestType.POST, user2Email);

			var obj = await RequestJSON($"{Dodo.DodoApp.API_ROOT}{ResourceRoot}/{group.Guid}", EHTTPRequestType.GET);
			Assert.IsNotNull(obj.Value<JObject>(nameof(GroupResource.AdministratorData).ToCamelCase()).Value<JArray>(nameof(GroupResource.AdminData.Administrators).ToCamelCase()).Values<JToken>()
				.Single(s => s.Value<string>(nameof(IResourceReference.Guid).ToCamelCase()).ToString() != user1.Guid.ToString()));
			await Logout();

			/*var inviteURL = EmailHelper.Callback;
			var trim = inviteURL.Substring(inviteURL.IndexOf("?token=") + 7);
			var user2Password = ValidationExtensions.GenerateStrongPassword();
			await Request(inviteURL, EHTTPRequestType.POST, new UserSchema("Test User 2", "testuser2", user2Password, user2Email));
			var user2 = UserManager.GetSingle(u => u.PersonalData.Email == user2Email);

			await Login(user2.Slug, user2Password);
			obj = await RequestJSON($"{DodoServer.DodoServer.API_ROOT}{ResourceRoot}/{group.Guid}", EHTTPRequestType.GET);
			Assert.AreEqual(PermissionLevel.ADMIN,
				obj.Value<JObject>(Resource.METADATA).Value<string>(Resource.METADATA_PERMISSION));

			Postman.Update(
				new PostmanEntryAddress { Category = PostmanCategory, Request = $"Add an Administrator by Email" },
				apiReq);*/
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
			Assert.AreEqual(verify[Resource.METADATA]["isMember"].Value<string>(), "true");

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
			Assert.AreEqual(verify[Resource.METADATA]["isMember"].Value<string>(), "false");

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
				var token = new SimpleNotificationToken(con.User, "Test", Message, true);
				notGuid = token.Guid;
				group.AddToken(token, EPermissionLevel.PUBLIC);
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
