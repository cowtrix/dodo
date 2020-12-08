using Common.Extensions;
using Dodo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Resources;
using System.Linq;
using System.Threading.Tasks;
using DodoTest.Framework.Postman;

namespace RESTTests
{
	public abstract class AdministratedGroupResourceTestBase<T, TSchema> : GroupResourceTestBase<T, TSchema>
		where T : AdministratedGroupResource
		where TSchema : DescribedResourceSchemaBase
	{
		#region Administration
		[TestMethod]
		[TestCategory("Administration")]
		public async Task Admin_CanAddAdminFromGuid()
		{
			// Create a new user
			var user1 = GetRandomUser(out var user1Password, out var user1Context);
			// Let them create a new group
			var group = CreateObject<T>(user1Context);
			Assert.IsTrue(group.IsAdmin(user1, user1Context, out var p));
			Assert.IsTrue(p.CanAddAdmin);
			await Login(user1.Slug, user1Password);

			var user2 = GetRandomUser(out var user2Password, out var user2Context);
			var apiReq = await Request($"{Dodo.DodoApp.API_ROOT}{ResourceRoot}/{group.Guid}/addadmin",
				EHTTPRequestType.POST, user2.Guid);

			var obj = await RequestJSON($"{Dodo.DodoApp.API_ROOT}{ResourceRoot}/{group.Guid}", EHTTPRequestType.GET);
			var adminData = obj.Value<JObject>(nameof(AdministratedGroupResource.AdministratorData).ToCamelCase());
			Assert.IsNotNull(adminData.Value<JArray>(nameof(AdministrationData.Administrators).ToCamelCase())
				.Values<JToken>().Single(s => s.Value<JToken>(nameof(AdministratorEntry.User).ToCamelCase()).
					Value<string>(nameof(IResourceReference.Guid).ToCamelCase()).ToString() == user2.Guid.ToString()));
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
		public async Task Admin_CanUpdateAdmin()
		{
			// Create a new user
			var user1 = GetRandomUser(out var user1Password, out var user1Context);
			var user2 = GetRandomUser(out var user2Password, out var user2Context);
			// Let them create a new group
			var group = CreateObject<T>(user1Context);
			// Add the second admin
			using (var rscLock = new ResourceLock(group))
			{
				group.AddNewAdmin(user1Context, user2);
				ResourceManager.Update(group, rscLock);
			}
			group = ResourceManager.GetSingle(rsc => rsc.Guid == group.Guid);
			var adminData = group.AdministratorData.GetValue(user1.CreateRef(), user1Context.Passphrase);
			Assert.IsFalse(adminData.Administrators.Single(ad => ad.User.Guid == user2.Guid).Permissions.CanEditInfo);
			await Login(user1.Slug, user1Password);
			await Request($"{Dodo.DodoApp.API_ROOT}{ResourceRoot}/{group.Slug}/updateadmin?id={user2.Slug}", EHTTPRequestType.POST, new AdministratorPermissionSet { CanEditInfo = true });
			group = ResourceManager.GetSingle(rsc => rsc.Guid == group.Guid);
			adminData = group.AdministratorData.GetValue(user1.CreateRef(), user1Context.Passphrase);
			Assert.IsTrue(adminData.Administrators.Single(ad => ad.User.Guid == user2.Guid).Permissions.CanEditInfo);
		}

		[TestMethod]
		[TestCategory("Administration")]
		public async Task Admin_CanRemoveAdmin()
		{
			// Create a new user
			var user1 = GetRandomUser(out var user1Password, out var user1Context);
			var user2 = GetRandomUser(out var user2Password, out var user2Context);
			// Let them create a new group
			var group = CreateObject<T>(user1Context);
			// Add the second admin
			using (var rscLock = new ResourceLock(group))
			{
				group.AddNewAdmin(user1Context, user2);
				ResourceManager.Update(group, rscLock);
			}
			await Login(user1.Slug, user1Password);
			// Remove the admin
			await Request($"{Dodo.DodoApp.API_ROOT}{ResourceRoot}/{group.Slug}/removeadmin?id={user2.Slug}", EHTTPRequestType.GET);
			group = ResourceManager.GetSingle(rsc => rsc.Guid == group.Guid);
			var adminData = group.AdministratorData.GetValue(user1.CreateRef(), user1Context.Passphrase);
			Assert.IsNull(adminData.Administrators.SingleOrDefault(ad => ad.User.Guid == user2.Guid));
		}

		[TestMethod]
		[TestCategory("Administration")]
		public async Task Admin_CanAddAdminFromEmail()
		{
			// Create a new user
			var user1 = GetRandomUser(out var user1Password, out var user1Context);
			// Let them create a new group
			var group = CreateObject<T>(user1Context);
			Assert.IsTrue(group.IsAdmin(user1, user1Context, out var p));
			Assert.IsTrue(p.CanAddAdmin);
			await Login(user1.Slug, user1Password);

			var user2Email = "myUser2@email.com";
			var apiReq = await Request($"{Dodo.DodoApp.API_ROOT}{ResourceRoot}/{group.Guid}/addadmin",
				EHTTPRequestType.POST, user2Email);

			await Logout();

			/*var inviteURL = EmailUtility.EmailHistory.Last().Contents.First().Value;
			var startUrlIndex = inviteURL.IndexOf(Dodo.DodoApp.NetConfig.FullURI);
			inviteURL = inviteURL.Substring(startUrlIndex);
			var user2Password = ValidationExtensions.GenerateStrongPassword();
			await Request(inviteURL, EHTTPRequestType.POST, new UserSchema("Test User 2", "testuser2", user2Password, user2Email));
			var user2 = UserManager.GetSingle(u => u.PersonalData.Email == user2Email);

			await Login(user2.Slug, user2Password);
			var obj = await RequestJSON($"{Dodo.DodoApp.API_ROOT}{ResourceRoot}/{group.Guid}", EHTTPRequestType.GET);
			Assert.AreEqual(PermissionLevel.ADMIN,
				obj.Value<JObject>(Resource.METADATA).Value<string>(Resource.METADATA_PERMISSION));

			Postman.Update(
				new PostmanEntryAddress { Category = PostmanCategory, Request = $"Add an Administrator by Email" },
				apiReq);*/
		}
		#endregion
	}
}
