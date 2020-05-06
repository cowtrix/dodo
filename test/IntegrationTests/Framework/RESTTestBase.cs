using System;
using System.Threading.Tasks;
using Common.Extensions;
using Dodo;
using Dodo.SharedTest;
using DodoTest.Framework.Postman;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Resources;

namespace RESTTests
{
	public abstract class RESTTestBase<T, TSchema> : IntegrationTestBase 
		where T:DodoResource
		where TSchema: ResourceSchemaBase
	{
		public abstract string ResourceRoot { get; }
		protected IResourceManager<T> ResourceManager => ResourceUtility.GetManager<T>();
		protected abstract string PostmanCategory { get; }

		[TestMethod]
		public async virtual Task CanGetAnonymously()
		{
			GetRandomUser(out _, out var context);
			var schema = SchemaGenerator.GetRandomSchema<T>(context) as TSchema;
			var resource = ResourceUtility.GetFactory<T>().CreateTypedObject(context, schema);
			var resourceObj = await RequestJSON($"{DodoServer.DodoServer.API_ROOT}{ResourceRoot}/{resource.Guid.ToString()}", EHTTPRequestType.GET);
			VerifyCreatedObject(resource, resourceObj, schema);

			Postman.Update(
				new PostmanEntryAddress { Category = PostmanCategory, Request = $"Get a {typeof(T).Name}" },
				LastRequest, 0, "Get anonymously");
		}

		[TestMethod]
		public async virtual Task CanGetAsOwner()
		{
			var user = GetRandomUser(out var password, out var context);
			var schema = SchemaGenerator.GetRandomSchema<T>(context) as TSchema;
			var resource = ResourceUtility.GetFactory<T>().CreateTypedObject(context, schema);
			await Login(user.AuthData.Username, password);
			var resourceObj = await RequestJSON($"{DodoServer.DodoServer.API_ROOT}{ResourceRoot}/{resource.Guid.ToString()}", EHTTPRequestType.GET);
			Assert.IsTrue(resourceObj[Resource.METADATA][Resource.METADATA_PERMISSION].Value<string>() == PermissionLevel.OWNER);

			Postman.Update(
				new PostmanEntryAddress { Category = PostmanCategory, Request = $"Get a {typeof(T).Name}" },
				LastRequest, 1, "Get as Owner");
		}

		[TestMethod]
		public async virtual Task CanGetAsUser()
		{
			var user = GetRandomUser(out var password, out var context);
			var resource = CreateObject<T>();
			await Login(user.AuthData.Username, password);
			var resourceObj = await RequestJSON($"{DodoServer.DodoServer.API_ROOT}{ResourceRoot}/{resource.Guid.ToString()}", EHTTPRequestType.GET);
			Assert.IsTrue(resourceObj[Resource.METADATA][Resource.METADATA_PERMISSION].Value<string>() == PermissionLevel.USER);

			Postman.Update(
				new PostmanEntryAddress { Category = PostmanCategory, Request = $"Get a {typeof(T).Name}" },
				LastRequest, 2, "Get as User");
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
			var response = await RequestJSON($"{DodoServer.DodoServer.API_ROOT}{ResourceRoot}", EHTTPRequestType.POST,
				SchemaGenerator.GetRandomSchema<T>(context));
			Postman.Update(
				new PostmanEntryAddress { Category = PostmanCategory, Request = $"Create a new {typeof(T).Name}" },
				LastRequest);
		}

		[TestMethod]
		public virtual async Task CanDelete()
		{
			var user = GetRandomUser(out var password, out var context);
			var resource = CreateObject<T>(context);
			await Login(user.AuthData.Username, password);
			await Request($"{DodoServer.DodoServer.API_ROOT}{ResourceRoot}/{resource.Guid.ToString()}", EHTTPRequestType.DELETE,
				SchemaGenerator.GetRandomSchema<T>(context));
			Assert.IsNull(ResourceManager.GetSingle(r => r.Guid == resource.Guid));
			Postman.Update(
				new PostmanEntryAddress { Category = PostmanCategory, Request = $"Delete a {typeof(T).Name}" },
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
			await RequestJSON($"{DodoServer.DodoServer.API_ROOT}{ResourceRoot}/{resource.Guid.ToString()}", EHTTPRequestType.PATCH,	patch);
			var updatedObj = ResourceManager.GetSingle(r => r.Guid == resource.Guid);
			VerifyPatchedObject(updatedObj, patch);
			Postman.Update(
				new PostmanEntryAddress { Category = PostmanCategory, Request = $"Update a {typeof(T).Name}" },
				LastRequest);
		}

		protected abstract void VerifyPatchedObject(T rsc, JObject patchObj);
	}
}
