using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Common;
using Common.Extensions;
using Dodo;
using Dodo.Rebellions;
using Dodo.SharedTest;
using Dodo.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Resources;

namespace RESTTests
{

	public abstract class RESTTestBase<T, TSchema> : IntegrationTestBase 
		where T:DodoResource
		where TSchema: DodoResourceSchemaBase
	{
		public abstract string ResourceRoot { get; }
		protected IResourceManager<T> ResourceManager => ResourceUtility.GetManager<T>();

		[TestMethod]
		public async virtual Task CanGetAnonymously()
		{
			GetRandomUser(out _, out var context);
			var schema = SchemaGenerator.GetRandomSchema<T>(context) as TSchema;
			var resource = ResourceUtility.GetFactory<T>().CreateTypedObject(context, schema);
			var resourceObj = await RequestJSON($"{ResourceRoot}/{resource.GUID.ToString()}", EHTTPRequestType.GET);
			VerifyCreatedObject(resource, resourceObj, schema);
		}

		protected virtual void VerifyCreatedObject(T rsc, JObject obj, TSchema schema)
		{
			Assert.AreEqual(rsc.GUID, Guid.Parse(obj.Value<string>("GUID")));
			Assert.AreEqual(rsc.Name, obj.Value<string>("Name"));
			Assert.AreEqual(rsc.Name, schema.Name);
		}

		[TestMethod]
		public virtual async Task CanCreate()
		{
			var user = GetRandomUser(out var password, out var context);
			await Login(user.AuthData.Username, password);
			var response = await RequestJSON(ResourceRoot, EHTTPRequestType.POST,
				SchemaGenerator.GetRandomSchema<T>(context));
		}

		[TestMethod]
		public virtual async Task CanDelete()
		{
			var user = GetRandomUser(out var password, out var context);
			var resource = CreateObject<T>(context);
			await Login(user.AuthData.Username, password);
			await Request($"{ResourceRoot}/{resource.GUID.ToString()}", EHTTPRequestType.DELETE,
				SchemaGenerator.GetRandomSchema<T>(context));
			Assert.IsNull(ResourceManager.GetSingle(r => r.GUID == resource.GUID));
		}

		protected abstract JObject GetPatchObject();

		[TestMethod]
		public virtual async Task CanPatch()
		{
			var user = GetRandomUser(out var password, out var context);
			var resource = CreateObject<T>(context);
			await Login(user.AuthData.Username, password);
			var patch = GetPatchObject();
			await RequestJSON($"{ResourceRoot}/{resource.GUID.ToString()}", EHTTPRequestType.PATCH,	patch);
			var updatedObj = ResourceManager.GetSingle(r => r.GUID == resource.GUID);
			VerifyPatchedObject(updatedObj, patch);
		}

		protected abstract void VerifyPatchedObject(T rsc, JObject patchObj);
	}
}
