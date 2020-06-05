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

namespace RESTTests
{
	public abstract class GroupResourceTestBase<T, TSchema> : RESTTestBase<T, TSchema> 
		where T:GroupResource
		where TSchema:OwnedResourceSchemaBase
	{
		[TestMethod]
		public override async Task CanCreate()
		{
			var user = GetRandomUser(out var password, out var context);
			using (var rscLock = new ResourceLock(user))
			{
				user.TokenCollection.Add(user, new ResourceCreationToken(typeof(T)));
				UserManager.Update(user, rscLock);
			}
			await Login(user.AuthData.Username, password);
			var schema = SchemaGenerator.GetRandomSchema<T>(context) as TSchema;
			var response = await RequestJSON($"{DodoServer.DodoServer.API_ROOT}{ResourceRoot}", EHTTPRequestType.POST, schema);
			user = UserManager.GetSingle(u => u.Guid == user.Guid);
			var rsc = ResourceManager.GetSingle(r => r.Guid.ToString() == response.Value<string>(nameof(IRESTResource.Guid).ToCamelCase()));
			VerifyCreatedObject(rsc, response, schema);

			Postman.Update(
				new PostmanEntryAddress { Category = PostmanCategory, Request = $"Create a new {PostmanTypeName}" },
				LastRequest);
		}

		[TestMethod]
		public async Task CreatorIsShownAsAndOwner()
		{
			var user = GetRandomUser(out var password, out var context);
			var group = CreateObject<T>(context);
			await Login(user.AuthData.Username, password);
			var obj = await RequestJSON($"{DodoServer.DodoServer.API_ROOT}{ResourceRoot}/{group.Guid}", EHTTPRequestType.GET);
			Assert.AreEqual(PermissionLevel.OWNER,
				obj.Value<JObject>(Resource.METADATA).Value<string>(Resource.METADATA_PERMISSION));
		}

		[TestMethod]
		public async Task CanAddAdminFromGuid()
		{
			// Create a new user
			var user1 = GetRandomUser(out var user1Password, out var user1Context);
			// Let them create a new group
			var group = CreateObject<T>(user1Context);
			Assert.IsTrue(group.IsAdmin(user1, user1Context));
			await Login(user1.AuthData.Username, user1Password);

			var user2 = GetRandomUser(out var user2Password, out var user2Context);
			var apiReq = await Request($"{DodoServer.DodoServer.API_ROOT}{ResourceRoot}/{group.Guid}/addadmin", 
				EHTTPRequestType.POST, user2.Guid);

			var obj = await RequestJSON($"{DodoServer.DodoServer.API_ROOT}{ResourceRoot}/{group.Guid}", EHTTPRequestType.GET);
			Assert.IsNotNull(obj.Value<JObject>(nameof(GroupResource.AdministratorData).ToCamelCase()).Value<JArray>(nameof(GroupResource.AdminData.Administrators).ToCamelCase()).Values<JToken>()
				.Single(s => s.Value<string>(nameof(IResourceReference.Guid).ToCamelCase()).ToString() == user2.Guid.ToString()));
			await Logout();

			await Login(user2.AuthData.Username, user2Password);
			obj = await RequestJSON($"{DodoServer.DodoServer.API_ROOT}{ResourceRoot}/{group.Guid}", EHTTPRequestType.GET);
			Assert.AreEqual(PermissionLevel.ADMIN,
				obj.Value<JObject>(Resource.METADATA).Value<string>(Resource.METADATA_PERMISSION));

			Postman.Update(
				new PostmanEntryAddress { Category = PostmanCategory, Request = $"Add an Administrator by Guid" },
				apiReq);
		}

		[TestMethod]
		public async Task CanAddAdminFromEmail()
		{
			Assert.Inconclusive();
			// Create a new user
			var user1 = GetRandomUser(out var user1Password, out var user1Context);
			// Let them create a new group
			var group = CreateObject<T>(user1Context);
			Assert.IsTrue(group.IsAdmin(user1, user1Context));
			await Login(user1.AuthData.Username, user1Password);

			var user2Email = "myUser2@email.com";
			var apiReq = await Request($"{DodoServer.DodoServer.API_ROOT}{ResourceRoot}/{group.Guid}/addadmin",
				EHTTPRequestType.POST, user2Email);

			var obj = await RequestJSON($"{DodoServer.DodoServer.API_ROOT}{ResourceRoot}/{group.Guid}", EHTTPRequestType.GET);
			Assert.IsNotNull(obj.Value<JObject>(nameof(GroupResource.AdministratorData).ToCamelCase()).Value<JArray>(nameof(GroupResource.AdminData.Administrators).ToCamelCase()).Values<JToken>()
				.Single(s => s.Value<string>(nameof(IResourceReference.Guid).ToCamelCase()).ToString() != user1.Guid.ToString()));
			await Logout();

			/*var inviteURL = EmailHelper.Callback;
			var trim = inviteURL.Substring(inviteURL.IndexOf("?token=") + 7);
			var user2Password = ValidationExtensions.GenerateStrongPassword();
			await Request(inviteURL, EHTTPRequestType.POST, new UserSchema("Test User 2", "testuser2", user2Password, user2Email));
			var user2 = UserManager.GetSingle(u => u.PersonalData.Email == user2Email);

			await Login(user2.AuthData.Username, user2Password);
			obj = await RequestJSON($"{DodoServer.DodoServer.API_ROOT}{ResourceRoot}/{group.Guid}", EHTTPRequestType.GET);
			Assert.AreEqual(PermissionLevel.ADMIN,
				obj.Value<JObject>(Resource.METADATA).Value<string>(Resource.METADATA_PERMISSION));

			Postman.Update(
				new PostmanEntryAddress { Category = PostmanCategory, Request = $"Add an Administrator by Email" },
				apiReq);*/
		}

		[TestMethod]
		public async Task CanJoin()
		{
			var group = CreateObject<T>();
			var user = GetRandomUser(out var password, out var context);
			await Login(user.AuthData.Username, password);
			var joinReq = await Request($"{DodoServer.DodoServer.API_ROOT}{ResourceRoot}/{group.Guid}/join", EHTTPRequestType.POST);
			var verify = await RequestJSON($"{DodoServer.DodoServer.API_ROOT}{ResourceRoot}/{group.Guid}", EHTTPRequestType.GET);
			Assert.AreEqual(verify[Resource.METADATA]["isMember"].Value<string>(), "true");

			Postman.Update(
				new PostmanEntryAddress { Category = PostmanCategory, Request = $"Join a {PostmanTypeName}" },
				joinReq);
		}

		[TestMethod]
		public async Task CanLeave()
		{
			var group = CreateObject<T>();
			var user = GetRandomUser(out var password, out var context);
			await Login(user.AuthData.Username, password);
			await Request($"{DodoServer.DodoServer.API_ROOT}{ResourceRoot}/{group.Guid}/join", EHTTPRequestType.POST);
			var leaveReq = await Request($"{DodoServer.DodoServer.API_ROOT}{ResourceRoot}/{group.Guid}/leave", EHTTPRequestType.POST);
			var verify = await RequestJSON($"{DodoServer.DodoServer.API_ROOT}{ResourceRoot}/{group.Guid}", EHTTPRequestType.GET);
			Assert.AreEqual(verify[Resource.METADATA]["isMember"].Value<string>(), "false");

			Postman.Update(
				new PostmanEntryAddress { Category = PostmanCategory, Request = $"Leave a {PostmanTypeName}" },
				leaveReq);
		}
	}
}
