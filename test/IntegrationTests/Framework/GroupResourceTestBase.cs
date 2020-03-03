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

namespace RESTTests
{
	public abstract class GroupResourceTestBase<T> : RESTTestBase<T> where T:GroupResource
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
			var list = await RequestJSON<JArray>($"{ResourceRoot}", EHTTPRequestType.GET,
				parameters: new[]
				{
					("latlong", $"{resource.Location.Latitude}+{resource.Location.Longitude}"),
					("distance", "20.6"),
				});
			var guids = list.Values<string>().Select(x => Guid.Parse(x)).GetEnumerator().ToIEnumerable().ToList();
			Assert.IsTrue(guids.Contains(resource.GUID));
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
			var list = await RequestJSON<JArray>($"{ResourceRoot}", EHTTPRequestType.GET,
				parameters: new[]
				{
					("startdate", $"{resource.StartDate.ToShortDateString()}"),
					("enddate", $"{resource.EndDate.ToShortDateString()}")
				});
			var guids = list.Values<string>().Select(x => Guid.Parse(x)).GetEnumerator().ToIEnumerable().ToList();
			Assert.IsTrue(guids.Contains(resource.GUID));
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
			var list = await RequestJSON<JArray>($"{ResourceRoot}", EHTTPRequestType.GET);
			var guids = list.Values<string>().Select(x => Guid.Parse(x)).GetEnumerator().ToIEnumerable().ToList();
			Assert.IsFalse(sites.Any(x => !guids.Contains(x.GUID)));
		}

		[TestMethod]
		public override async Task CanCreate()
		{
			var user = GetRandomUser(out var password, out var context);
			using (var rscLock = new ResourceLock(user))
			{
				user.TokenCollection.Add(new ResourceCreationToken(typeof(T)));
				UserManager.Update(user, rscLock);
			}
			await Login(user.AuthData.Username, password);
			var response = await RequestJSON(ResourceRoot, EHTTPRequestType.POST,
				SchemaGenerator.GetRandomSchema<T>(context));
			user = UserManager.GetSingle(u => u.GUID == user.GUID);
			Assert.IsTrue(user.TokenCollection.GetTokens<ResourceCreationToken>().Single().IsRedeemed);
		}

		[TestMethod]
		public async Task CreatorIsShownAsAndOwner()
		{
			var user = GetRandomUser(out var password, out var context);
			var group = CreateObject<T>(context);
			await Login(user.AuthData.Username, password);
			var obj = await RequestJSON($"{ResourceRoot}/{group.GUID}", EHTTPRequestType.GET);
			Assert.AreEqual(PermissionLevel.OWNER,
				obj.Value<JObject>(Resource.METADATA).Value<string>(Resource.METADATA_PERMISSION));
		}

		[TestMethod]
		public async Task CanAddAdminFromExistingUser()
		{
			// Create a new user
			var user1 = GetRandomUser(out var user1Password, out var user1Context);
			// Let them create a new group
			var group = CreateObject<T>(user1Context);
			Assert.IsTrue(group.IsAdmin(user1, user1Context));
			await Login(user1.AuthData.Username, user1Password);

			var user2 = GetRandomUser(out var user2Password, out var user2Context);
			await Request($"{ResourceRoot}/{group.GUID}/addadmin", 
				EHTTPRequestType.POST, user2.GUID);
			var obj = await RequestJSON($"{ResourceRoot}/{group.GUID}", EHTTPRequestType.GET);
			Assert.IsNotNull(obj.Value<JObject>("AdministratorData").Value<JArray>("Administrators").Values<JToken>()
				.Single(s => s.Value<string>("guid").ToString() == user2.GUID.ToString()));
			await Logout();

			await Login(user2.AuthData.Username, user2Password);
			obj = await RequestJSON($"{ResourceRoot}/{group.GUID}", EHTTPRequestType.GET);
			Assert.AreEqual(PermissionLevel.ADMIN,
				obj.Value<JObject>(Resource.METADATA).Value<string>(Resource.METADATA_PERMISSION));
		}
		/*
		[TestMethod]
		public void CanAddAdminFromNewUser()
		{
			var createdObj = RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			var resourceURL = createdObj.Value<string>("ResourceURL");

			var secondEmail = "test@blahnotawebsite.com.ul";
			var addAdminResponse = Request(resourceURL + GroupResourceRESTHandler.ADD_ADMIN, Method.POST, secondEmail);
			Assert.IsTrue(addAdminResponse.StatusCode == System.Net.HttpStatusCode.OK);

			var secondPass = ValidationExtensions.GenerateStrongPassword();
			var secondUser = RegisterUser(out var guid, username: "seconduser", email: secondEmail, password: secondPass);
			createdObj = RequestJSON(resourceURL, Method.GET);
			var admin = createdObj.Value<JObject>("AdministratorData").Value<JArray>("Administrators").AsJEnumerable().Select(x => x.Value<string>("Guid"));
			Assert.IsNotNull(admin.SingleOrDefault(x => x == guid));
		}

		[TestMethod]
		public void AdminsCanSeeOtherAdmins()
		{
			var createdObj = RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			var resourceURL = createdObj.Value<string>("ResourceURL");

			RegisterRandomUser(out var username1, out _, out var password, out _, out var guid);
			Request(resourceURL + GroupResourceController.ADD_ADMIN, Method.POST, guid);

			var updatedObj = RequestJSON(resourceURL, Method.GET);
			var admin = updatedObj.Value<JObject>("AdministratorData").Value<JArray>("Administrators").AsJEnumerable().Select(x => x.Value<string>("Guid"));
			Assert.IsNotNull(admin.SingleOrDefault(x => x == guid));
		}

		[TestMethod]
		public void CannotCreateWhenNotVerified()
		{
			var unverifiedUser = RegisterRandomUser(out var username, out _, out var pass, out _, out _, false);
			AssertX.Throws<Exception>(() => RequestJSON(CreationURL, Method.POST, GetCreationSchema(), username, pass),
				e => e.Message.Contains("You need to verify your email"));
		}

		[TestMethod]
		public void CannotPatchWhenNotVerified()
		{
			var createdObj = RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			var unverifiedUser = RegisterRandomUser(out var username, out _, out var pass, out _, out _, false);
			AssertX.Throws<Exception>(() => RequestJSON(createdObj.Value<string>("ResourceURL"), Method.PATCH, GetPatchSchema(), username, pass),
				e => e.Message.Contains("Forbidden"));
		}

		[TestMethod]
		public void CannotDeleteWhenNotVerified()
		{
			var createdObj = RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			var unverifiedUser = RegisterRandomUser(out var username, out _, out var pass, out _, out _, false);
			AssertX.Throws<Exception>(() => RequestJSON(createdObj.Value<string>("ResourceURL"), Method.PATCH, GetPatchSchema(), username, pass),
				e => e.Message.Contains("Forbidden"));
		}

		[TestMethod]
		public void CannotEditIfNotAdmin()
		{
			var createdObj = RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			var resourceURL = createdObj.Value<string>("ResourceURL");

			RegisterRandomUser(out var username1, out _, out var password, out _, out var guid);

			AssertX.Throws<Exception>(() => RequestJSON(resourceURL, Method.PATCH, GetPatchSchema(), username1, password),
				e => e.Message.Contains("Forbidden"));

			AssertX.Throws<Exception>(() => RequestJSON(resourceURL, Method.DELETE, GetPatchSchema(), username1, password),
				e => e.Message.Contains("Forbidden"));
		}

		[TestMethod]
		public void CanGetIfNotAdmin()
		{
			var createdObj = RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			var resourceURL = createdObj.Value<string>("ResourceURL");

			var response = RequestJSON(resourceURL, Method.GET, null, "", "");

			RegisterRandomUser(out var username1, out _, out var password, out _, out var guid);
			response = RequestJSON(resourceURL, Method.GET, null, username1, password);
		}

		[TestMethod]
		public void CanJoinAndLeave()
		{
			var createdObj = RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			var resourceURL = createdObj.Value<string>("ResourceURL");
			var newUser = RegisterRandomUser(out var username, out _, out var password, out _, out _);
			createdObj = RequestJSON(resourceURL, Method.GET, user: username, password:password);
			Assert.AreEqual("false", createdObj.Value<string>(GroupResource.IS_MEMBER_AUX_TOKEN));
			Request(resourceURL + GroupResourceRESTHandler<T>.JOIN_GROUP, Method.POST,
				username: username, password: password);
			createdObj = RequestJSON(resourceURL, Method.GET, user: username, password: password);
			Assert.AreEqual("true", createdObj.Value<string>(GroupResource.IS_MEMBER_AUX_TOKEN));
			Request(resourceURL + GroupResourceRESTHandler<T>.LEAVE_GROUP, Method.POST,
				username: username, password: password);
			createdObj = RequestJSON(resourceURL, Method.GET, user: username, password: password);
			Assert.AreEqual("false", createdObj.Value<string>(GroupResource.IS_MEMBER_AUX_TOKEN));
		}

		[TestMethod]
		public void CanJoinAndLeaveMultithread()
		{
			var createdObj = RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			var resourceURL = createdObj.Value<string>("ResourceURL");
			int joinCounter = 0;
			object counterLock = new object();
			var joinAction = new Action<EventWaitHandle>(handle =>
			{
				var newUser = RegisterRandomUser(out var username, out _, out var password, out _, out _);
				createdObj = RequestJSON(resourceURL, Method.GET, user: username, password: password);
				var isMember = createdObj.Value<string>(GroupResource.IS_MEMBER_AUX_TOKEN);
				Assert.AreEqual("false", isMember);
				// Join
				Request(resourceURL + GroupResourceRESTHandler<T>.JOIN_GROUP, Method.POST,
					username: username, password: password);
				createdObj = RequestJSON(resourceURL, Method.GET, user: username, password: password);
				isMember = createdObj.Value<string>(GroupResource.IS_MEMBER_AUX_TOKEN);
				Assert.AreEqual("true", isMember);
				lock(counterLock)
				{
					joinCounter++;
				}
				// Leave
				Request(resourceURL + GroupResourceRESTHandler<T>.LEAVE_GROUP, Method.POST,
					username: username, password: password);
				lock (counterLock)
				{
					joinCounter--;
				}
				createdObj = RequestJSON(resourceURL, Method.GET, user: username, password: password);
				isMember = createdObj.Value<string>(GroupResource.IS_MEMBER_AUX_TOKEN);
				Assert.AreEqual("false", isMember);
				handle.Set();
			});
			const int taskCount = 64;
			var waitHandles = new WaitHandle[taskCount];
			for(var i = 0; i < taskCount; ++i)
			{
				var handle = new EventWaitHandle(false, EventResetMode.ManualReset);
				var thread = new Thread(() => joinAction(handle));
				waitHandles[i] = handle;
				thread.Start();
			}
			WaitHandle.WaitAll(waitHandles);
			Assert.AreEqual(0, joinCounter);
		}*/
	}
}
