using Common.Extensions;
using Dodo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using RestSharp;
using Resources;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SharedTest;
using Dodo.Rebellions;
using Dodo.SharedTest;
using System.Collections.Generic;
using Dodo.Users;
using Dodo.Users.Tokens;
using DodoResources;
using DodoTest.Framework.Postman;
using Resources.Security;
using Dodo.Utility;

namespace RESTTests
{
	public abstract class GroupResourceTestBase<T, TSchema> : RESTTestBase<T, TSchema> 
		where T:GroupResource
		where TSchema:GroupResourceSchemaBase
	{
		[TestMethod]
		public async virtual Task CanListWithDistanceFilter()
		{
			GetRandomUser(out _, out var context);
			var factory = ResourceUtility.GetFactory<T>();
			var resources = new List<T>();
			for (var i = 0; i < 5; ++i)
			{
				resources.Add(factory.CreateTypedObject(context, SchemaGenerator.GetRandomSchema<T>(context)));
			}
			var resource = resources.Random() as ILocationalResource;
			if (resource == null)
			{
				Assert.Inconclusive();
			}
			var list = await RequestJSON<JArray>($"{DodoServer.DodoServer.API_ROOT}{ResourceRoot}", EHTTPRequestType.GET,
				parameters: new[]
				{
					(nameof(DistanceFilter.latlong), $"{resource.Location.Latitude}+{resource.Location.Longitude}"),
					(nameof(DistanceFilter.distance), "20.6"),
				});
			var guids = list.Values<JObject>().Select(o => o.Value<string>(nameof(IRESTResource.Guid).ToCamelCase()));
			Assert.IsTrue(guids.Contains(resource.Guid.ToString()));
		}

		[TestMethod]
		public async virtual Task CanListWithDateFilter()
		{
			GetRandomUser(out _, out var context);
			var factory = ResourceUtility.GetFactory<T>();
			var resources = new List<T>();
			for (var i = 0; i < 1; ++i)
			{
				resources.Add(factory.CreateTypedObject(context, SchemaGenerator.GetRandomSchema<T>(context)));
			}
			var resource = resources.Random() as ITimeBoundResource;
			if(resource == null)
			{
				Assert.Inconclusive();
			}
			var list = await RequestJSON<JArray>($"{DodoServer.DodoServer.API_ROOT}{ResourceRoot}", EHTTPRequestType.GET,
				parameters: new[]
				{
					(nameof(DateFilter.startdate), $"{resource.StartDate.ToShortDateString()}"),
					(nameof(DateFilter.enddate), $"{resource.EndDate.ToShortDateString()}")
				});
			var guids = list.Values<JObject>().Select(o => o.Value<string>(nameof(IRESTResource.Guid).ToCamelCase()));
			Assert.IsTrue(guids.Contains(resource.Guid.ToString()));
		}

		[TestMethod]
		public async virtual Task CanList()
		{
			GetRandomUser(out _, out var context);
			var factory = ResourceUtility.GetFactory<T>();
			var sites = new List<T>();
			for (var i = 0; i < 5; ++i)
			{
				sites.Add(factory.CreateTypedObject(context, SchemaGenerator.GetRandomSchema<T>(context)));
			}
			var list = await RequestJSON<JArray>($"{DodoServer.DodoServer.API_ROOT}{ResourceRoot}", EHTTPRequestType.GET);
			var guids = list.Values<JObject>().Select(o => o.Value<string>(nameof(IRESTResource.Guid).ToCamelCase()));
			Assert.IsFalse(sites.Any(x => !guids.Contains(x.Guid.ToString())));

			Postman.Update(
				new PostmanEntryAddress { Category = PostmanCategory, Request = $"List all {typeof(T).Name}s" },
				LastRequest);
		}

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
			//Assert.IsTrue(user.TokenCollection.GetTokens<ResourceCreationToken>().Single().IsRedeemed);
			var rsc = ResourceManager.GetSingle(r => r.Guid.ToString() == response.Value<string>(nameof(IRESTResource.Guid).ToCamelCase()));
			VerifyCreatedObject(rsc, response, schema);

			Postman.Update(
				new PostmanEntryAddress { Category = PostmanCategory, Request = $"Create a new {typeof(T).Name}" },
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
				new PostmanEntryAddress { Category = PostmanCategory, Request = $"Join a {typeof(T).Name}" },
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
				new PostmanEntryAddress { Category = PostmanCategory, Request = $"Leave a {typeof(T).Name}" },
				leaveReq);
		}
	}
}
