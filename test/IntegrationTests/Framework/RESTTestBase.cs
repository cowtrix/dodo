using System;
using System.Threading.Tasks;
using Common.Extensions;
using Dodo;
using Dodo.SharedTest;
using DodoTest.Framework.Postman;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Resources;
using SharedTest;

namespace RESTTests
{
	public abstract class RESTTestBase<T, TSchema> : IntegrationTestBase 
		where T:DodoResource
		where TSchema: ResourceSchemaBase
	{
		public string PostmanTypeName => typeof(T).Name;
		public abstract string ResourceRoot { get; }
		protected virtual IResourceManager<T> ResourceManager => ResourceUtility.GetManager<T>();
		protected abstract string PostmanCategory { get; }

		[TestMethod]
		public async virtual Task CanGetWithGuidAnonymously()
		{
			GetRandomUser(out _, out var context);
			var schema = SchemaGenerator.GetRandomSchema<T>(context) as TSchema;
			var resource = CreateObject<T>(context, schema);
			var resourceObj = await RequestJSON($"{Dodo.DodoServer.API_ROOT}{ResourceRoot}/{resource.Guid.ToString()}", EHTTPRequestType.GET);
			VerifyCreatedObject(resource, resourceObj, schema);

			Postman.Update(
				new PostmanEntryAddress { Category = PostmanCategory, Request = $"Get a {PostmanTypeName}" },
				LastRequest, 0, "Get anonymously");
		}

		[TestMethod]
		public async virtual Task CanGetWithSlugAnonymously()
		{
			GetRandomUser(out _, out var context);
			var schema = SchemaGenerator.GetRandomSchema<T>(context) as TSchema;
			var resource = CreateObject<T>(context, schema);
			var resourceObj = await RequestJSON($"{Dodo.DodoServer.API_ROOT}{ResourceRoot}/{resource.Slug}", EHTTPRequestType.GET);
			VerifyCreatedObject(resource, resourceObj, schema);
		}

		[TestMethod]
		public async virtual Task CanGetWithGuidAsOwner()
		{
			var user = GetRandomUser(out var password, out var context);
			var schema = SchemaGenerator.GetRandomSchema<T>(context) as TSchema;
			var resource = CreateObject<T>(context, schema);
			await Login(user.AuthData.Username, password);
			var resourceObj = await RequestJSON($"{Dodo.DodoServer.API_ROOT}{ResourceRoot}/{resource.Guid.ToString()}", EHTTPRequestType.GET);
			Assert.IsTrue(resourceObj[Resource.METADATA][Resource.METADATA_PERMISSION].Value<string>() == PermissionLevel.OWNER);

			Postman.Update(
				new PostmanEntryAddress { Category = PostmanCategory, Request = $"Get a {PostmanTypeName}" },
				LastRequest, 2, "Get as Owner");
		}

		[TestMethod]
		public async virtual Task CanGetWithSlugAsOwner()
		{
			var user = GetRandomUser(out var password, out var context);
			var schema = SchemaGenerator.GetRandomSchema<T>(context) as TSchema;
			var resource = CreateObject<T>(context, schema);
			await Login(user.AuthData.Username, password);
			var resourceObj = await RequestJSON($"{Dodo.DodoServer.API_ROOT}{ResourceRoot}/{resource.Slug}", EHTTPRequestType.GET);
			Assert.IsTrue(resourceObj[Resource.METADATA][Resource.METADATA_PERMISSION].Value<string>() == PermissionLevel.OWNER);
		}

		[TestMethod]
		public async virtual Task CanGetWithGuidAsUser()
		{
			var user = GetRandomUser(out var password, out var context);
			var resource = CreateObject<T>();
			await Login(user.AuthData.Username, password);
			var resourceObj = await RequestJSON($"{Dodo.DodoServer.API_ROOT}{ResourceRoot}/{resource.Guid}", EHTTPRequestType.GET);
			Assert.IsTrue(resourceObj[Resource.METADATA][Resource.METADATA_PERMISSION].Value<string>() == PermissionLevel.USER);

			Postman.Update(
				new PostmanEntryAddress { Category = PostmanCategory, Request = $"Get a {PostmanTypeName}" },
				LastRequest, 2, "Get as User");
		}

		[TestMethod]
		public async virtual Task CanGetWithSlugAsUser()
		{
			var user = GetRandomUser(out var password, out var context);
			var resource = CreateObject<T>();
			await Login(user.AuthData.Username, password);
			var resourceObj = await RequestJSON($"{Dodo.DodoServer.API_ROOT}{ResourceRoot}/{resource.Slug}", EHTTPRequestType.GET);
			Assert.IsTrue(resourceObj[Resource.METADATA][Resource.METADATA_PERMISSION].Value<string>() == PermissionLevel.USER);
		}

		[TestMethod]
		public async virtual Task BadGetWithGuidReturns404()
		{
			await AssertX.ThrowsAsync<Exception>(Request($"{Dodo.DodoServer.API_ROOT}{ResourceRoot}/{Guid.NewGuid()}", EHTTPRequestType.GET),
				e => e.Message.Contains("Not Found"));
		}

		[TestMethod]
		public async virtual Task BadGetWithSlugReturns404()
		{
			await AssertX.ThrowsAsync<Exception>(Request($"{Dodo.DodoServer.API_ROOT}{ResourceRoot}/thisisabadslug", EHTTPRequestType.GET),
				e => e.Message.Contains("Not Found"));
		}

		protected virtual void VerifyCreatedObject(T rsc, JObject obj, TSchema schema)
		{
			Assert.AreEqual(rsc.Guid, Guid.Parse(obj.Value<string>(nameof(IRESTResource.Guid).ToCamelCase())));
			Assert.AreEqual(rsc.Name, obj.Value<string>(nameof(IRESTResource.Name).ToCamelCase()));
			Assert.AreEqual(rsc.Name, schema.Name);
		}

		[TestMethod]
		public virtual async Task CanCreate()
		{
			var user = GetRandomUser(out var password, out var context);
			await Login(user.AuthData.Username, password);
			var response = await RequestJSON($"{Dodo.DodoServer.API_ROOT}{ResourceRoot}", EHTTPRequestType.POST,
				SchemaGenerator.GetRandomSchema<T>(context));
			Postman.Update(
				new PostmanEntryAddress { Category = PostmanCategory, Request = $"Create a new {PostmanTypeName}" },
				LastRequest);
		}

		[TestMethod]
		public virtual async Task CanDelete()
		{
			var user = GetRandomUser(out var password, out var context);
			var resource = CreateObject<T>(context);
			await Login(user.AuthData.Username, password);
			await Request($"{Dodo.DodoServer.API_ROOT}{ResourceRoot}/{resource.Guid.ToString()}", EHTTPRequestType.DELETE,
				SchemaGenerator.GetRandomSchema<T>(context));
			Assert.IsNull(ResourceManager.GetSingle(r => r.Guid == resource.Guid));
			Postman.Update(
				new PostmanEntryAddress { Category = PostmanCategory, Request = $"Delete a {PostmanTypeName}" },
				LastRequest);
		}

		protected abstract JObject GetPatchObject();

		[TestMethod]
		public virtual async Task CanPatch()
		{
			var user = GetRandomUser(out var password, out var context);
			var resource = CreateObject<T>(context);
			await Login(user.AuthData.Username, password);
			var patch = GetPatchObject();
			await RequestJSON($"{Dodo.DodoServer.API_ROOT}{ResourceRoot}/{resource.Guid.ToString()}", EHTTPRequestType.PATCH,	patch);
			var updatedObj = ResourceManager.GetSingle(r => r.Guid == resource.Guid);
			VerifyPatchedObject(updatedObj, patch);
			Postman.Update(
				new PostmanEntryAddress { Category = PostmanCategory, Request = $"Update a {PostmanTypeName}" },
				LastRequest);
		}

		[TestMethod]
		public virtual async Task CanPublish()
		{
			var user = GetRandomUser(out var password, out var context);
			var resource = CreateObject<T>(context) as IPublicResource;
			resource.Publish();
			resource = ResourceManager.GetSingle(rsc => rsc.Guid == resource.Guid) as IPublicResource;
			Assert.IsTrue(resource.IsPublished);
		}

		[TestMethod]
		public void CanGetResourceManager() => Assert.IsNotNull(ResourceManager);

		protected abstract void VerifyPatchedObject(T rsc, JObject patchObj);
	}
}
